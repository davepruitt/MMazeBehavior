   M S B A M L   `   `   `  ����H  DMMazeBehavior, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:MMazeBehaviorMMazeBehavior  5   	   [ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms 5   	   ^ ZAccord.Controls.Imaging, Version=3.8.0.0, Culture=neutral, PublicKeyToken=fa1a88e29555ccf7R>clr-namespace:Accord.Controls;assembly=Accord.Controls.ImagingAccord.Controls 5   	       MMazeBehavior.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:MMazeBehavior     5   	   Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     5   	   Maf>clr-namespace:Accord.Controls;assembly=Accord.Controls.Imaging     5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5	   	   -   5          � WindowState$  	Maximized=�5   	    � Title$ M-Maze Behavior��5   	   $	��800��6!   $
��1200��6.   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��	Rat Name:��6!   $��24(�62   �� 5   
   $��1��6   $��0��6   $��2��6/   $��60(�5!      $��5q�6    $��Center=�5"      $	��100��5#      ��5       ��  �� Path$ RatName� �� FallbackValue$	 Rat��{�5!   +   �� $ IsEditingEnabled�$
 True��5#      �� 5%   
   $��0��6   $��2��6!   $��Stage:��61   $��24(�6?   �� 5&   
   $��1��6   $��2��6    $��2��60   $��42(�6D   $��5q�6R   $	��100��5'      $��Center=�5(      ��5)      �� $ 	StageList� �� SelectedIndex 5*      �� $ SelectedStageIndex�{�5+      �� $ IsEditingEnabled�$
 True���� 5.   
   $��2��6   $��0��6!   $��2��61   $��48(�6E   $��5q�6S   $
��Left=�6^   $��Center=�5/      ��6/   �� $ 	BoothName�$ Booth���� 50   
   $��2��6   $��2��6!   $��2��61   $��48(�6E   $��5q�6S   $
��Left=�6^   $��Center=�51      ��6/   �� $ SessionTimerText�$ Timer��� 54   
   $��3��6   $��0��6   $��4��6,    55      �� 56      $��* �6#   �� 57      $��* �6#   �� 58      $��* �6#   .+�5;      �� $��0��6    �� Orientation$ 
Horizontal=�6)   $��Center=�6B   .+�5<      �� $��Left feeds: ��6   $��48(�5=      �� 5?      $��48(�5@      ��5?      �� $ 	LeftFeeds�$ 0��5@      5B      �� 5C      $��1��6   $ 
Horizontal=�6)   $��Center=�6B   .+�5D      �� $��Right feeds: ��6   $��48(�5E      �� 5G      $��48(�5H      ��5G      �� $ 
RightFeeds�$ 0��5H      5J      �� 5K      $��2��6   $ 
Horizontal=�6)   $��Center=�6B   .+�5L      �� $��Total feeds: ��6   $��48(�5M      �� 5O      $��48(�5P      ��5O      �� $ 
TotalFeeds�$ 0��5P      5R      5T      �� 5W   
   -   5        $��4��5W      $��0��6   $��4��6.   $��Center=�6B   $	��600��5X      $	��100��6   $��5q�5Y      $��Center=�6   $��60(�5Z      $
��Bold"�6   ��5\      �� $ StartButtonContent�$ Start����5]      �� $ StartButtonColor�$ Green��{�5^      �� $ IsStartButtonEnabled�]� 5a   
   ��. $��5��6)   $��0��66   $��2��6F   $��10q�6Z   .+�5b      7  1System.Windows.Forms.Integration.WindowsFormsHost  $��Stretch=�6   $��Stretch=�6=   $	��Top=�5c      y�5d      �� $ CameraVisibility�$ Visible��   Child. 5e      '  !Accord.Controls.VideoSourcePlayer  -   5        CameraVideoSourcePlayer��5e   '   5g      5h      �� 5j   
   $��5��6   $��0��6!   $��2��61   $��10q�6E   $��Center=�5k      $��Center=�61   $"��Unable to connect to camera!��5m      y�5l      �� $" CameraNotAvailableVisibility�$ 	Collapsed��5m        OxyPlot.Wpf.PlotView  5p   
   $��5��6   $��2��5q      $��2��5r      $��Stretch=�5s      $��Stretch=�5t      $��10q�5u      	   Model	 5v      �� $ SessionPlotModel�5x      5y      