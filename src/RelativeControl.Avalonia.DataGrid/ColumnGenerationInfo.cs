using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Utils;
using RelativeControl.Avalonia.DataGrid.Utils;
using AvaloniaDataGrid = Avalonia.Controls.DataGrid;

namespace RelativeControl.Avalonia.DataGrid;

public enum ColumnGenerationStatus {
    Off, Waiting, Generating, Done
}

public enum RelativeApplyStatus {
    Waiting, Applying, Done
}

public sealed class DataGridColumnGenerationStatusChangedEventArgs(
    ColumnGenerationStatus oldValue,
    ColumnGenerationStatus newValue) : EventArgs {
    public readonly ColumnGenerationStatus NewValue = newValue;
    public readonly ColumnGenerationStatus OldValue = oldValue;
}

public sealed class DataGridRelativeApplyStatusChangedEventArgs(
    AvaloniaProperty property,
    RelativeApplyStatus oldValue,
    RelativeApplyStatus newValue) : EventArgs {
    public readonly RelativeApplyStatus NewValue = newValue;
    public readonly RelativeApplyStatus OldValue = oldValue;
    public readonly AvaloniaProperty Property = property;
}

public class ColumnGenerationInfo {
    private readonly AvaloniaDataGrid _dataGrid;
    private int _totalColumns;

    public ColumnGenerationInfo(AvaloniaDataGrid dataGrid) {
        _dataGrid = dataGrid;
        Status = _dataGrid.AutoGenerateColumns ? ColumnGenerationStatus.Waiting : ColumnGenerationStatus.Off;
        RelativeWidthApplyStatus = RelativeApplyStatus.Waiting;
        RelativeMinWidthApplyStatus = RelativeApplyStatus.Waiting;
        RelativeMaxWidthApplyStatus = RelativeApplyStatus.Waiting;
        if (_dataGrid.AutoGenerateColumns)
            _dataGrid.Loaded += BeginUpdate;
        else
            _totalColumns = _dataGrid.Columns.Count;
        _dataGrid.PropertyChanged += (_, args) => {
            if (args.Property == AvaloniaDataGrid.ItemsSourceProperty)
                UpdateTotalColumns();

            if (args.Property != AvaloniaDataGrid.AutoGenerateColumnsProperty)
                return;
            if (args.NewValue is true) {
                if (_dataGrid.IsLoaded) {
                    _dataGrid.Loaded -= BeginUpdate;
                    BeginUpdate();
                } else {
                    _dataGrid.Loaded -= BeginUpdate;
                    _dataGrid.Loaded += BeginUpdate;
                }
            } else
                StopUpdate();
        };
    }

    public ColumnGenerationStatus Status { get; private set; }
    public RelativeApplyStatus RelativeWidthApplyStatus { get; private set; }
    public RelativeApplyStatus RelativeMinWidthApplyStatus { get; private set; }
    public RelativeApplyStatus RelativeMaxWidthApplyStatus { get; private set; }
    public event EventHandler<DataGridColumnGenerationStatusChangedEventArgs>? ColumnsGenerationStatusChanged;
    public event EventHandler<DataGridRelativeApplyStatusChangedEventArgs>? RelativeApplyStatusChanged;

    internal void ChangeRelativeApplyStatus(AvaloniaProperty property, RelativeApplyStatus newStatus) {
        if (RelativeWidthApplyStatus == newStatus)
            return;
        RelativeApplyStatus oldStatus;
        if (property == RelativeDataGrid.ColumnWidthsProperty) {
            oldStatus = RelativeWidthApplyStatus;
            RelativeWidthApplyStatus = newStatus;
        } else if (property == RelativeDataGrid.MinColumnWidthsProperty) {
            oldStatus = RelativeMinWidthApplyStatus;
            RelativeMinWidthApplyStatus = newStatus;
        } else if (property == RelativeDataGrid.MaxColumnWidthsProperty) {
            oldStatus = RelativeMaxWidthApplyStatus;
            RelativeMaxWidthApplyStatus = newStatus;
        } else {
            throw new InvalidOperationException("Unknown property");
        }

        RelativeApplyStatusChanged?.Invoke(
            this,
            new DataGridRelativeApplyStatusChangedEventArgs(property, oldStatus, newStatus));
    }

    private void BeginUpdate(object? _1 = null, object? _2 = null) {
        UpdateTotalColumns();
        UpdateStatus();
        _dataGrid.Columns.CollectionChanged += UpdateStatus;
    }

    private void StopUpdate() {
        _totalColumns = -1;
        ChangeStatusTo(ColumnGenerationStatus.Off);
        _dataGrid.Columns.CollectionChanged -= UpdateStatus;
    }

    private void UpdateStatus(object? _1 = null, object? _2 = null) {
        Debug.Assert(_dataGrid.AutoGenerateColumns);
        if (_dataGrid.Columns.Count == 0) {
            ChangeStatusTo(ColumnGenerationStatus.Waiting);
            return;
        }

        if (_dataGrid.Columns.Count == _totalColumns) {
            ChangeStatusTo(ColumnGenerationStatus.Done);
            return;
        }

        ChangeStatusTo(ColumnGenerationStatus.Generating);
    }

    private void ChangeStatusTo(ColumnGenerationStatus newStatus) {
        if (Status == newStatus)
            return;
        ColumnGenerationStatus oldStatus = Status;
        Status = newStatus;
        ColumnsGenerationStatusChanged?.Invoke(
            this,
            new DataGridColumnGenerationStatusChangedEventArgs(oldStatus, newStatus));
    }

    private void UpdateTotalColumns() {
        int? totalColumns = _dataGrid.ItemsSource.GetItemType()
                                     ?.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Length;
        Debug.Assert(totalColumns != null);
        _totalColumns = (int)totalColumns;
    }

    public int GetTotalColumns() { return _dataGrid.AutoGenerateColumns ? _totalColumns : _dataGrid.Columns.Count; }
}