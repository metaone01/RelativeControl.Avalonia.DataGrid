# RelativeControl.Avalonia.DataGrid

This provides some relative units and features for [Avalonia DataGrid](https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid).

[中文](README_CN.md)

[See Usages in Demo](https://github.com/metaone01/RelativeControl.Avalonia.DataGrid/tree/main/Demo.RelativeControl.DataGrid/Demo.RelativeControl.DataGrid/MainWindow.axaml)

## Get Started

### Add NuGet package:

```bash
dotnet add package RelativeControl.Avalonia.DataGrid
```

## Before Use

Note that if user resizes a column, it will stay the size user changes.
Re-set its `RelativeDataGrid.WidthProperty` can recover the responsiveness.

> [!IMPORTANT]
> We have to set `IsRelativeEnabled="True"` to the `DataGrid` to enable enhanced relative features.
> This is a must. There are so many `internal`s in Avalonia Source Code,
> so we have to use this setter to initialize some critical refs.

> [!IMPORTANT]
> `DataGridColumn` is not a visual component.
> So when we calculate its relative source, we use its owning datagrid.
> It means that `sw` unit while setting column widths means its owning `DataGrid`'s width.
> Other units are also calculated from its owning `DataGrid`.

## Use It On Your DataGrid

```xml

<DataGrid ItemSource="{Binding YourItemsSource}" AutoGenerateColumns="True"
          rdg:RelativeDataGrid.IsRelativeEnabled="True"
          rdg:RelativeDataGrid.ColumnWidths="[-1]50px [0]100px [5]200px"/>
```

> [!WARNING]
> When setting `ColumnWidths`'s value(`[INDEX]VALUE`),there must have no whitespace between `[INDEX]` and `VALUE`.

The `ColumnWidths` property allows you to set relative widths for auto-generated columns.
Specially, the width starts with `[-1]` will apply to all columns.

## Use It On Your DataGrid Columns

```xml

<DataGrid>
    <DataGrid rdg:RelativeDataGrid.IsRelativeEnabled="True">
        <DataGrid.Columns>
            <DataGridTextColumn rdg:RelativeDataGrid.Width="20sw"/>
        </DataGrid.Columns>
    </DataGrid>
</DataGrid>
```