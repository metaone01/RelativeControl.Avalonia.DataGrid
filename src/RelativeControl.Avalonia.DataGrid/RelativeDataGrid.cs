using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaDataGrid = Avalonia.Controls.DataGrid;

namespace RelativeControl.Avalonia.DataGrid;

public class RelativeDataGrid : AvaloniaObject {
    public static readonly AttachedProperty<ColumnGenerationInfo?> ColumnGenerationInfoProperty =
        AvaloniaProperty.RegisterAttached<Relative, AvaloniaDataGrid, ColumnGenerationInfo?>("ColumnGenerationInfo");

    public static readonly AttachedProperty<Dictionary<int, IRelative<double>>> ColumnWidthsProperty =
        AvaloniaProperty.RegisterAttached<Relative, AvaloniaDataGrid, Dictionary<int, IRelative<double>>>(
            "ColumnWidths",
            new Dictionary<int, IRelative<double>>());

    public static readonly AttachedProperty<Dictionary<int, IRelative<double>>> MinColumnWidthsProperty =
        AvaloniaProperty.RegisterAttached<Relative, AvaloniaDataGrid, Dictionary<int, IRelative<double>>>(
            "MinColumnWidths",
            new Dictionary<int, IRelative<double>>());

    public static readonly AttachedProperty<Dictionary<int, IRelative<double>>> MaxColumnWidthsProperty =
        AvaloniaProperty.RegisterAttached<Relative, AvaloniaDataGrid, Dictionary<int, IRelative<double>>>(
            "MaxColumnWidths",
            new Dictionary<int, IRelative<double>>());

    public static readonly AttachedProperty<IRelative<double>> WidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, DataGridColumn, IRelative<double>>("Width", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, DataGridColumn, IRelative<double>>(
            "MinWidth",
            RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, DataGridColumn, IRelative<double>>(
            "MaxWidth",
            RelativeLength.PositiveInfinity);

    public static readonly AttachedProperty<AvaloniaDataGrid?> OwningGridProperty =
        AvaloniaProperty.RegisterAttached<Relative, DataGridColumn, AvaloniaDataGrid?>("OwningGrid");

    static RelativeDataGrid() {
        WidthProperty.Changed.AddClassHandler<DataGridColumn>((column, args) => {
            switch (args.NewValue) {
                case RelativeExpression expression:
                    column.SetValue(args.Property, RelativeLengthBase.Parse(expression.Expression, column));
                    break;
                case IRelative<double> nl: {
                    column.SetValue(DataGridColumn.WidthProperty, new DataGridLength(nl.Absolute()));
                    WeakReference<DataGridColumn> reference = new(column);

                    nl.RelativeChanged -= Update;
                    nl.RelativeChanged += Update;
                    break;

                    void Update(IRelative<double>? sender, RelativeChangedEventArgs<double> changed) {
                        if (!reference.TryGetTarget(out DataGridColumn? target))
                            return;
                        if (target.CanUserResize && Math.Abs(target.Width.Value - changed.OldValue) > 1e-5) {
                            return;
                        }

                        target.SetValue(DataGridColumn.WidthProperty, new DataGridLength(sender!.Absolute()));
                    }
                }
                case null:
                    break;
                default:
                    throw new InvalidOperationException($"{args.NewValue.GetType()} is not a correct type.");
            }
        });
        MinWidthProperty.Changed.AddClassHandler<DataGridColumn>((column, args) => {
            switch (args.NewValue) {
                case RelativeExpression expression:
                    column.SetValue(args.Property, RelativeLengthBase.Parse(expression.Expression, column));
                    break;
                case IRelative<double> nv:
                    column.MinWidth = nv.Absolute();
                    var reference = new WeakReference<DataGridColumn>(column);
                    nv.RelativeChanged -= Update;
                    nv.RelativeChanged += Update;
                    break;

                    void Update(IRelative<double>? sender, RelativeChangedEventArgs<double> _) {
                        if (reference.TryGetTarget(out DataGridColumn? target))
                            target.MinWidth = sender!.Absolute();
                    }
            }
        });
        MaxWidthProperty.Changed.AddClassHandler<DataGridColumn>((column, args) => {
            switch (args.NewValue) {
                case RelativeExpression expression:
                    column.SetValue(args.Property, RelativeLengthBase.Parse(expression.Expression, column));
                    break;
                case IRelative<double> nv:
                    var reference = new WeakReference<DataGridColumn>(column);
                    nv.RelativeChanged -= Update;
                    nv.RelativeChanged += Update;
                    column.MaxWidth = nv.Absolute();
                    break;

                    void Update(IRelative<double>? sender, RelativeChangedEventArgs<double> _) {
                        if (reference.TryGetTarget(out DataGridColumn? target))
                            target.MaxWidth = sender!.Absolute();
                    }
            }
        });
        OwningGridProperty.Changed.AddClassHandler<DataGridColumn>((column, args) => {
            if (GetWidth(column) is RelativeLength width) {
                width.SetVisualAnchor(args.NewValue as Visual);
            }

            if (GetMinWidth(column) is RelativeLengthBase minWidth) {
                minWidth.SetVisualAnchor(args.NewValue as Visual);
            }

            if (GetMaxWidth(column) is RelativeLengthBase maxWidth) {
                maxWidth.SetVisualAnchor(args.NewValue as Visual);
            }
        });
    }

    private static void RelativeDataGridPropertyChangedHandler(AvaloniaDataGrid dataGrid, AvaloniaProperty property) {
        if (GetColumnGenerationInfo(dataGrid) is { } info) {
            info.ColumnsGenerationStatusChanged -= Update;
            info.ColumnsGenerationStatusChanged += Update;
        } else if (!dataGrid.AutoGenerateColumns) {
            Update(
                null,
                new DataGridColumnGenerationStatusChangedEventArgs(
                    ColumnGenerationStatus.Off,
                    ColumnGenerationStatus.Off));
        }

        return;

        void Update(object? _1, DataGridColumnGenerationStatusChangedEventArgs args) {
            if (args.NewValue is not ColumnGenerationStatus.Done and not ColumnGenerationStatus.Off)
                return;
            ColumnGenerationInfo info1 = GetColumnGenerationInfo(dataGrid)!;
            info1.ChangeRelativeApplyStatus(property, RelativeApplyStatus.Applying);
            Func<AvaloniaDataGrid, Dictionary<int, IRelative<double>>> getter;
            Action<DataGridColumn, object> setter;
            if (property == ColumnWidthsProperty) {
                getter = GetColumnWidths;
                setter = SetWidth;
            } else if (property == MinColumnWidthsProperty) {
                getter = GetMinColumnWidths;
                setter = SetMinWidth;
            } else if (property == MaxColumnWidthsProperty) {
                getter = GetMaxColumnWidths;
                setter = SetMaxWidth;
            } else {
                throw new InvalidOperationException($"Property {property} not supported.");
            }

            Dictionary<int, IRelative<double>> values = getter(dataGrid);
            IRelative<double> defaultValue = values.GetValueOrDefault(-1, RelativeLength.Empty);
            Dispatcher.UIThread.InvokeAsync(() => {
                foreach (var (column, index) in dataGrid.Columns.Select((column, index) => (column, index)))
                    setter(column, values.GetValueOrDefault(index, defaultValue));

                info1.ChangeRelativeApplyStatus(property, RelativeApplyStatus.Done);
                info1.ColumnsGenerationStatusChanged -= Update;
            });
        }
    }

    private static Dictionary<int, IRelative<double>> ParseToRelatives(string s, AvaloniaDataGrid dataGrid) {
        return Splitters.Split(s, ',', ' ')
                        .AsParallel()
                        .Select((x, index) => {
                            x = x.Trim();
                            if (x.StartsWith('[') && x.IndexOf(']') is var val and > 0)
                                return (relative: x[(val + 1)..], index: Convert.ToInt32(x[1..val]));

                            return (relative: x, index);
                        })
                        .ToDictionary(
                            val => val.index,
                            IRelative<double> (val) => RelativeLengthBase.Parse(val.relative, dataGrid));
    }

    public static void SetIsRelativeEnabled(AvaloniaDataGrid dataGrid, bool isEnable) {
        if (isEnable) {
            if (dataGrid.GetValue(ColumnGenerationInfoProperty) is null)
                dataGrid.SetValue(ColumnGenerationInfoProperty, new ColumnGenerationInfo(dataGrid));
            if (!dataGrid.IsLoaded)
                dataGrid.Loaded += InitializeOwningGrid;
            else
                InitializeOwningGrid(null, EventArgs.Empty);
        } else {
            dataGrid.SetValue(ColumnGenerationInfoProperty, null);
            dataGrid.Columns.CollectionChanged -= UpdateOwningGrid;
            foreach (DataGridColumn column in dataGrid.Columns)
                column.SetValue(OwningGridProperty, null);
        }

        return;

        void UpdateOwningGrid(object? sender, NotifyCollectionChangedEventArgs args) {
            if (args.NewItems is null)
                return;
            foreach (DataGridColumn col in args.NewItems)
                col.SetValue(OwningGridProperty, dataGrid);
        }

        void InitializeOwningGrid(object? sender, EventArgs args) {
            foreach (DataGridColumn col in dataGrid.Columns)
                col.SetValue(OwningGridProperty, dataGrid);
            dataGrid.Columns.CollectionChanged -= UpdateOwningGrid;
            dataGrid.Columns.CollectionChanged += UpdateOwningGrid;
        }
    }

    /// <summary>
    ///     使用[-1]表示所有列
    /// </summary>
    public static void SetColumnWidths(AvaloniaDataGrid dataGrid, object length) {
        if (GetColumnGenerationInfo(dataGrid) is null) {
            throw new RelativeNotEnabledException(
                "RelativeDataGrid is not enabled. Please call SetIsEnableRelative method first.");
        }

        Dictionary<int, IRelative<double>> values = length switch {
            RelativeExpression expression => ParseToRelatives(expression.Expression, dataGrid),
            string s => ParseToRelatives(s, dataGrid),
            RelativeLengthBase l => new Dictionary<int, IRelative<double>> { [-1] = l },
            Dictionary<int, IRelative<double>> dict => dict,
            _ => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
        };
        if (!dataGrid.IsLoaded)
            dataGrid.Loaded += Update;

        Update();
        return;

        void Update(object? _1 = null, object? _2 = null) {
            dataGrid.SetValue(ColumnWidthsProperty, values);
            RelativeDataGridPropertyChangedHandler(dataGrid, ColumnWidthsProperty);
            dataGrid.Loaded -= Update;
        }
    }

    /// <summary>
    ///     使用[-1]表示所有列
    /// </summary>
    public static void SetMinColumnWidths(AvaloniaDataGrid dataGrid, object length) {
        if (GetColumnGenerationInfo(dataGrid) is null) {
            throw new RelativeNotEnabledException(
                "RelativeDataGrid is not enabled. Please call SetIsEnableRelative method first.");
        }

        Dictionary<int, IRelative<double>> values = length switch {
            RelativeExpression expression => ParseToRelatives(expression.Expression, dataGrid),
            string s => ParseToRelatives(s, dataGrid),
            RelativeLengthBase l => new Dictionary<int, IRelative<double>> { [-1] = l },
            Dictionary<int, IRelative<double>> dict => dict,
            _ => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
        };
        if (!dataGrid.IsLoaded)
            dataGrid.Loaded += Update;

        Update();
        return;

        void Update(object? _1 = null, object? _2 = null) {
            dataGrid.SetValue(MinColumnWidthsProperty, values);
            RelativeDataGridPropertyChangedHandler(dataGrid, MinColumnWidthsProperty);
            dataGrid.Loaded -= Update;
        }
    }

    /// <summary>
    ///     使用[-1]表示所有列
    /// </summary>
    public static void SetMaxColumnWidths(AvaloniaDataGrid dataGrid, object length) {
        if (GetColumnGenerationInfo(dataGrid) is null) {
            throw new RelativeNotEnabledException(
                "RelativeDataGrid is not enabled. Please call SetIsEnableRelative method first.");
        }

        Dictionary<int, IRelative<double>> values = length switch {
            RelativeExpression expression => ParseToRelatives(expression.Expression, dataGrid),
            string s => ParseToRelatives(s, dataGrid),
            RelativeLengthBase l => new Dictionary<int, IRelative<double>> { [-1] = l },
            Dictionary<int, IRelative<double>> dict => dict,
            _ => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
        };
        if (!dataGrid.IsLoaded)
            dataGrid.Loaded += Update;

        Update();
        return;

        void Update(object? _1 = null, object? _2 = null) {
            dataGrid.SetValue(MaxColumnWidthsProperty, values);
            RelativeDataGridPropertyChangedHandler(dataGrid, MaxColumnWidthsProperty);
            dataGrid.Loaded -= Update;
        }
    }

    [Pure]
    public static Dictionary<int, IRelative<double>> GetColumnWidths(AvaloniaDataGrid dataGrid) {
        return dataGrid.GetValue(ColumnWidthsProperty);
    }

    [Pure]
    public static Dictionary<int, IRelative<double>> GetMinColumnWidths(AvaloniaDataGrid dataGrid) {
        return dataGrid.GetValue(MinColumnWidthsProperty);
    }

    [Pure]
    public static Dictionary<int, IRelative<double>> GetMaxColumnWidths(AvaloniaDataGrid dataGrid) {
        return dataGrid.GetValue(MaxColumnWidthsProperty);
    }

    [Pure]
    public static ColumnGenerationInfo? GetColumnGenerationInfo(AvaloniaDataGrid dataGrid) {
        return dataGrid.GetValue(ColumnGenerationInfoProperty);
    }

    public static void SetWidth(DataGridColumn column, object length) {
        column.SetValue(
            WidthProperty,
            length switch {
                RelativeExpression expression => RelativeLengthBase.Parse(
                    expression.Expression,
                    column,
                    column.GetValue(OwningGridProperty)),
                string s             => RelativeLengthBase.Parse(s, column, column.GetValue(OwningGridProperty)),
                RelativeLengthBase l => l,
                _                    => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetWidth(DataGridColumn column) { return column.GetValue(WidthProperty); }

    public static void SetMinWidth(DataGridColumn column, object length) {
        column.SetValue(
            MinWidthProperty,
            length switch {
                RelativeExpression expression => RelativeLengthBase.Parse(
                    expression.Expression,
                    column,
                    column.GetValue(OwningGridProperty)),
                string s             => RelativeLengthBase.Parse(s, column, column.GetValue(OwningGridProperty)),
                RelativeLengthBase l => l,
                _                    => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetMinWidth(DataGridColumn column) { return column.GetValue(MinWidthProperty); }

    public static void SetMaxWidth(DataGridColumn column, object length) {
        column.SetValue(
            MaxWidthProperty,
            length switch {
                RelativeExpression expression => RelativeLengthBase.Parse(
                    expression.Expression,
                    column,
                    column.GetValue(OwningGridProperty)),
                string s             => RelativeLengthBase.Parse(s, column, column.GetValue(OwningGridProperty)),
                RelativeLengthBase l => l,
                _                    => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetMaxWidth(DataGridColumn column) { return column.GetValue(MaxWidthProperty); }

    public static void UpdateColumnWidths(AvaloniaDataGrid dataGrid) {
        foreach (DataGridColumn? column in dataGrid.Columns) {
            if (GetWidth(column) is not RelativeLengthBase relativeWidth)
                return;
            relativeWidth.SetVisualAnchor(column.GetValue(OwningGridProperty));
        }
    }
}

public class RelativeNotEnabledException(string? message = null) : InvalidOperationException(message);