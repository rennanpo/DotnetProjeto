   M S B A M L   `   `   `  ����  MultiScreenApp"    MultiScreenApp.DisplayWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  DisplayWindow��5   	   $	��600��6   $	��800��6,    � WindowStyle$
 None=�5   	    � AllowsTransparency$
 True��6   
������� � 	Resources 5       �� 
TargetType �� Property �� Value��%V  '��       'r� �     �� 5	   
   " ��6   .�5
      ��  � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       � ��$ Uniform=�6(   5      �� 5   
   " r�6   .�5      ��  � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       �@r�Stretch$ Uniform=�6(   5      5      .��5      � -   5        $��MainGrid��5      
���   � � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5        $��DisplayedImage1��5      $y�	Collapsed=�6'   $��0��6>   $��2��6N   �� 5   
   -   5        $��DisplayedImage2��5      $y�	Collapsed=�6'   $��1��6>   r� 5   
   -   5        $��DisplayedVideo��5      $y�	Collapsed=�6-    r� LoadedBehavior$ Manual=�6k   	 r� UnloadedBehavior$
	 Stop=�6�   $��2��6�   5      5      