   M S B A M L   `   `   `  ????H  DPluginUpdater, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:PluginUpdaterPluginUpdater  5   	   #    PluginUpdater.MainApplication   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:PluginUpdater     5   	     ? WindowStartupLocation$  CenterScreen=?5   	    ? Title$ Plugin updater??5	   	   $
??Auto??5
   	   $	??200??5   	   $	??510??5   	   $	??510??5   	    ? SizeToContent$ Height=?5   	   
???????y?5   	   ?? *ApplicationVisibility+ ?? Mode$ TwoWay=? ?? UpdateSourceTrigger$ PropertyChanged=? ? 	Resources 5       ?? MergedDictionaries ?? Source-   'PluginUpdater.BoolToVisibilityConverter   boolToVisibilityConverter4   .PluginUpdater.InverseBoolToVisibilityConverter $  inverseBoolToVisibilityConverter*   $PluginUpdater.StatusToColorConverter  statusToColorConverter,   &PluginUpdater.StatusToMessageConverter  statusToMessageConverter(   "PluginUpdater.BoolInverseConverter  boolInverseConverter?? 5   
    5      ?? 5      $: 4/PluginUpdater;component/Controls/ControlStyles.xaml??6%   %?   &	        &	      &	 &     &	 9     &	 L       5      6.     5      65     5      6+     5      6-     5      6)   5      .??5       ?  ? RowDefinitions 5!   
   ?? 5"      $??* ?6   ?? 5#      $
??Auto ?6   $??40??5$      .+?5&   
   ?? $
??Auto=?5'      $??Disabled=?5(      .??5*      ?? $??Stretch=?5+      	 ?? Padding$	 5q?5,      $
??Wrap=?5.      $??10q?5/      ??5*      ?? *WarningMessage+y?50      ?? *
IsProgress+
 ?? 	Converter#
 [  ??51      ?? 52       ?? 
TargetType" ??6    ?? Triggers 53      ?? 54       ?? Binding 6*   ?? *IsFail+ ?? Value True6E   .a?55   "   ??  ?? Property ? ?? ?? Value$
 Bold"?6@   ?? 56   "    ? ??$ 12(?6>   ?? 57   "    ? ??
 ?????6?   ?? 58   "    ? ??
 ? E??6?   59      ?? 5:       6*   ?? *IsFail+	 False6E   .a?5;   "   ??  ? ??$ Medium"?6@   ?? 5<   "    ? ??$ 16(?6>   ?? 5=   "    ? ??
 ?????6?   ?? 5>   "    ? ??
 ???? 6?   5?      5A      5C      5D      ?? 5E   
   $
??Auto=?5F      $??Disabled=?5G      .??5I      ?? $??0??5J      $??5q?5K      ??5L      ?? *	Plugins+y?5M      ?? *
IsProgress+#
 [ ??5N      ?? 5O      .R?5P      ? -   5         ? Cursor$
 Hand??5Q   !   $??2q?5S   !   ??5T   !   ?? *Status+#
 [  ? ColumnDefinitions 5U      ?? 5V   "   $
??Auto ?63   ?? 5W   "   $??* ?63   .+?5Y      ? $??0??5Z   %   $
??Auto??5[   %   .+?5\   "   ?? $??20??5]   %   ${?False??5^   %   $??5q?5_   %   $??Center=?5`   %   $??Center=?5a   %    d? 	IsChecked 5b   %   ?? *	Checked+5c      ? 5d      $??1??5e   %   .+?5f   "   ? $??0??5g   %   .+?5h   &   ?  5i   *   ?? 5j   .   $
??Auto ?6?   ?? 5k   .   $??* ?6?   ?? 5l   .   $
??Auto ?6?   .+?5n   *   ??  ?? Orientation$ 
Horizontal=?65   $??Center=?5o   5   .+?5p   .   ?? $	??5,0q?5r   1   $
??Bold"?5s   1   $	??ID:??5t   1   ?? 5u   .   $??0??5v   1   $	??100??5w   1   $	??5,0q?5x   1   $??CharacterEllipsis=?5y   1    ? ToolTip 5z   1   ?? *ID+??5{   1   ??  ?? Path$ ID??? 5}   .   $
??Bold"?5~   1    ?? TextAlignment$ Center=?5   1   $	??5,0q?5?   1   $??Весрия:??5?   1   ?? 5?   .   $ Center=?5?   1   ??5?   1   ?? *	Version+5?   +   ?? 5?   *   $??2??5?   -   $??5,5,10,5q?5?   -   $
??Bold"?5?   -   $??Center=?5?   -   ??5?   -   ?? *Status+#
 [ 5?   '   5?   #   5?      5?      5?      5?      5?      ? 5?   
   $??1??5?      .+?5?      ?? $	??150??5?      $
??Left=?5?      $??5q?5?      $??Bottom=?5?      $??30??5?        ButtonLightStyle#??[ 5?      $??Продолжить??5?      
????·?{?5?      ?? *
IsProgress+#
 [ ??5?      ?? *ApplayCommand+63   5?      5?      5?      