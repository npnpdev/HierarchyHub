   M S B A M L   `   `   `  ����  PT_Lab1     PT_Lab1.CreatePersonWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Create Person��5   	   $	��200��6   $	��300��6,    � WindowStartupLocation$ CenterOwner=�5   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      �� 5	      �� 5
      �� 5      $
��Auto �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��0,0,0,5q�6;   .+�5      �� $��Name:��6   $��70��6$   �� 5      -   5         ��Name$ NameTextBox��5      $	��180��6+   5      �� 5   
   $ 
Horizontal=�6   $��1��6.   $��0,0,0,5q�6;   .+�5      �� $��Surname:��6   $��70��6'   �� 5      -   5        $ SurnameTextBox��5      $	��180��6.   5      �� 5   
   $ 
Horizontal=�6   $��2��6.   $��0,0,0,5q�6;   .+�5      �� $
��Age:��6   $��70��6#   �� 5      -   5        $ 
AgeTextBox��5      $	��180��6*   5      �� 5   
   $ 
Horizontal=�6   $��3��6.   $��Right=�6;   $��0,10,0,0q�6W   .+�5      �� -   5        $��Generate��5      $	��5,0q�6?   �� 5      -   5        $��OK��5       �� 	IsDefault . $	��5,0q�6D   �� 5      $��Cancel��6    �� IsCancel . $	��5,0q�66   5      5       5!      