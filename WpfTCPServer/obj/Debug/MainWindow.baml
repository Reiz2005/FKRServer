   M S B A M L   `   `   `  ����G  CWpfTCPServer, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null+clr-namespace:WpfTCPServerWpfTCPServer  5   	       WpfTCPServer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WpfTCPServer     5   	   -   5          � Title$  TCPServer WPF��5   	   $	��600��6   $	��800��6,    � WindowStartupLocation$ CenterScreen=�68    � 
ResizeMode$ CanMinimize=�6]   .��5	      � $��5q�6    � RowDefinitions 5
   
   �� 5      $��30 �6   �� 5      $��40 �6   �� 5      $��30 �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��383* �6   �� 5      $	��21* �6   �� 5      $
��386* �6   .+�5   
   �� $��16(�6   $��0��6   $��0��6+   .��6;   TCP服务器配置面板6G   �� 5   
   -   5         ��Name$ BtnStart��5      $��1��6#   $��0��60   .��6W   启动服务器6^   �� 5   
   $��1��6   $��2��6   .��6-   每隔多少秒保存日志:69   �� 5   
   -   5         ��Name$ saveSec_TextBox��5   /   $��1��6   $��2��6   $��30��6H   $��123,5,163,5q�6T   $��30��6i   $��Center=�6s   $��Center=�6�   �� 5   
   $��0��6   $��2��6   .��6-   $"服务器端口设置(默认5000):6?   �� 5   
   -   5        $ servPort_TextBox��5      $��0��6,   $��2��69   $��167,0,107,0q�6I    �� AcceptsReturn .   �� TextWrapping$ NoWrap=�6t   $��Center=�6�   $��Center=�6�   �� 5   
   $��2��6   $��0��6   .��6-   服务器日志:65   �� 5   
   $��2��6   $��2��6   .��6-   客户端管理功能66   �� 5   
   -   5        $ servLog_TextBox��5      $��3��6+   $��0��68   $	��450��6H    . 	 �� 
AcceptsTab	 . $
 Wrap=�6|   $
 �� HorizontalScrollBarVisibility$

 Auto=�6�   " �� VerticalScrollBarVisibility$
 Auto=�6�   �� 5!   
   -   5         ��Name$ clientsList��5!      $��3��6'   $��2��64    � ContextMenu 5"      �� 5#      .@�5$      m� -   5        $��发送消息��5$      6J   m� 5%      -   5        $��获取截图��5%      6M   m� 5&      -	   5        $��管理进程��5&      6M   m� 5'      -
   5        $��关机��5'      6L   m� 5(      -   5        $��重启��5(      6K   m� 5)      -   5        $��静音��5)      6H   m� 5*      -   5        $	��CMD��5*      6H   m� 5+      -   5        $��断开连接��5+      6Q   5,      5.      50      51      