   M S B A M L   `   `   `  ����A  =Spectr, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Spectr.WindowsSpectr.Windows  5   	   &     Spectr.Windows.AddCategoryWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Spectr.Windows     5   	     � Title$  AddCategoryWindow��5   	   $	��300��6#   $	��800��60   .��5	      �  � RowDefinitions 5
   
   �� 5      $	��10* �6   6+   �� 5      $��3* �6   6*   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��CategoriesDataGrid��5      $��0��5      $��3��5      $��10,10,10,10q�5      $	��200��5      $
��1000��5      $��Stretch=�5      $��Stretch=�5         AutoGenerateColumns .     SelectionMode$ Single=�5         CanUserAddRows .     
IsReadOnly .    SelectionUnit$ FullRow=�5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$
	 Auto 6P   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 61   �� *
CategoryID+6P     5      $ Название��6%   $
	 Auto 6T   
 67   �� *
Category+6T   5      �� 5!   
   -   5        $��1��5!      $��Добавить��6   $��15(�61   $��10q�6?   6Y   5"      5#      