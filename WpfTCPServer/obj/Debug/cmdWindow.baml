   M S B A M L   `   `   `  ����G  CWpfTCPServer, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null+clr-namespace:WpfTCPServerWpfTCPServer  5   	       WpfTCPServer.cmdWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WpfTCPServer     5   	     � Title$  	cmdWindow��5   	   $	��450��6   $	��800��6(   .��5	      � $��5q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ label_title��5      $��0��6   .��62   客户端CMD回显6<   �� 5   
   -   5         ��Name$ cmdBox��5      $��1��6"   $	��340��6/    �� AcceptsReturn .  �� 
AcceptsTab .  �� TextWrapping$
 Wrap=�6c   " �� VerticalScrollBarVisibility$
 Auto=�6w   $ �� HorizontalScrollBarVisibility$
 Auto=�6�   6�   �� 5   
   -   5        $ commandLine��5      $��2��6'    .   .  $��30��6]   $	��650��6i   $��5q�6u   $
��Left=�6�   6�   �� 5   
   -   5        	 ��Name$	 btnSend��5      $��2��6"   $��Right=�6/   $��5q�6K   $��70��6V   .��6w   执行6{   5      5      