   M S B A M L   `   `   `  ����G  CWpfTCPServer, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null+clr-namespace:WpfTCPServerWpfTCPServer  5   	   $    WpfTCPServer.sendMessageDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WpfTCPServer     5   	     � Title$  sendMessageDialog��5   	   $	��300��6#   $	��500��60    � 
ResizeMode$ CanMinimize=�6<    � WindowStartupLocation$ CenterScreen=�6U   .��5	      � $��3q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   .��6   要说的道理6$   �� 5   
   -   5         ��Name$ textBox��5   S   $��1��6   $	��200��6    �� AcceptsReturn .  �� 
AcceptsTab . 6f   �� 5   
   -   5         ��Name$ checkBox��5   U   $��2��6   $
��Auto��6    $��Right=�6-   $��10q�6I   .��6g   使客户端弹出信息框6r   �� 5   
   -   5        $��2��5      $	��100��6   $
��Left=�6*   $��10q�6E   .��6j   发送6n   �� 5   
   -   5        $��2��5      $	��100��6   $��Center=�6*   $��10q�6G   .��6n   取消6r   5      5      