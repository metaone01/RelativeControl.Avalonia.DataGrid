# RelativeControl.Avalonia.DataGrid

本包为 [Avalonia DataGrid](https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid) 提供一些相对单位的功能。

[English](README.md)

[在Demo中查看用例](https://github.com/metaone01/RelativeControl.Avalonia.DataGrid/tree/main/Demo.RelativeControl.DataGrid/Demo.RelativeControl.DataGrid/MainWindow.axaml)

## 开始使用

### 添加NuGet包

```bash
dotnet add package RelativeControl.Avalonia.DataGrid
```

## 使用前注意

> [!NOTE]
> 如果用户调整了列宽，则会保持用户的设定。
> 重新设定`RelativeDataGrid.WidthProperty`可以恢复响应性。

> [!IMPORTANT]
> 必须设定 `IsRelativeEnabled="True"` 来启用 `DataGrid` 的相对单位功能。
> 这是必要的。在Avalonia源码中有太多的 `internal`，
> 所以我们需要使用这个函数来初始化一些重要的引用。

> [!IMPORTANT]
> `DataGridColumn` 不是一个Visual控件。
> 所以在搜索相对源时，使用它的父`DataGrid`作为视觉锚点。
> 这意味着设定列宽时，单位`sw`意味着其父 `DataGrid` 的宽度。
> 其他单位同样以`DataGrid`算起。

## 在DataGrid中使用

```xml

<DataGrid ItemSource="{Binding YourItemsSource}" AutoGenerateColumns="True"
          rdg:RelativeDataGrid.IsRelativeEnabled="True"
          rdg:RelativeDataGrid.ColumnWidths="[-1]50px [0]100px [5]200px"/>
```

> [!WARNING]
> 当设定`ColumnWidths`的值(`[INDEX]VALUE`)时,在`[INDEX]`和`VALUE`之间不允许存在空格.

`ColumnWidths`属性可以为自动生成的列应用相对单位。
特殊地，索引为`[-1]`的值会应用到所有列。

## 在DataGrid Column中使用

```xml

<DataGrid>
    <DataGrid rdg:RelativeDataGrid.IsRelativeEnabled="True">
        <DataGrid.Columns>
            <DataGridTextColumn rdg:RelativeDataGrid.Width="20sw"/>
        </DataGrid.Columns>
    </DataGrid>
</DataGrid>
```