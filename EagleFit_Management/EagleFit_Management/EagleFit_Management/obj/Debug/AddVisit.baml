   M S B A M L   `   `   `  ����  EagleFit_Management,    &EagleFit_Management.AddActivityOrVisit   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$$  EagleFit Management: Add Visit��5   	   $	��575��60   $	��700��6=   
�������.��5      � .+�5   
   o� $
��Left=�6   $��20��6*   $	��Top=�66   $	��692��6N   .@�5      m� $
��File��6   .@�5      m� -   5        $��Close��5      $
��Left=�6*   $	��145��6E   6Q   5	      m� 5
      -   5        $
��Help��5
      6%   5      �� 5   
   -   5         ��Name$ lbl_studentID��5      $��
Student ID��6'   $
��Left=�6<   $��	25,40,0,0q�6W   $	��Top=�6j   �� 5   
   -   5         ��Name$ txtBx_studentID��5      $
��Left=�6+   $��23��6F   $��	25,66,0,0q�6R    �� TextWrapping$
 Wrap=�6e   $	��Top=�6y   $	��120��6�    �� 	MaxLength$ 8��6�    �� MaxLines$ 1��6�   �� 5   
   -   5         ��Name$ 	btn_Query��5      $��Query��6$   $
��Left=�64   $��
168,67,0,0q�6O   $	��Top=�6c   $��75��6{   6�   �� 5   
   -   5        $ lbl_idNotFound��5      $�� ��6(   $
��Left=�63   $��	25,94,0,0q�6N   $	��Top=�6a   $	��120��6y   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dgv_visitData��5      $
��Left=�6*   $��
25,125,0,0q�6E   $	��Top=�6Y   $	��206��6q   $	��641��6~   �� 5   
   -   5        $ btn_addNewVisit��5      $��Add New Visit��6*   $
��Left=�6B   $��
25,350,0,0q�6]   $	��Top=�6q   $	��100��6�   6�   �� 5   
   -	   5        $ txtBx_visitNum��5      $
��Left=�6*   $��23��6E   $��521,350,0,0q�6Q   $
 Wrap=�6f   $	��Top=�6z   $��50��6�   $ 1��6�   �� 5   
   -
   5        $ btn_editVisit��5      $��
Edit Visit��6(   $
��Left=�6=   $��591,350,0,0q�6X   $	��Top=�6m   $��75��6�   6�   �� 5   
   -   5        $ lbl_editVisit��5      $��Enter Visit to Edit: ��6'   $
��Left=�6G   $��410,350,0,0q�6b   $	��Top=�6w   $	��111��6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ grpBx_editVisit��5      $��Add/Edit Visit��6,   $
��Left=�6D   $��
25,387,0,0q�6_   $	��Top=�6s   $	��138��6�   $	��641��6�   5      (  "System.Windows.Controls.DatePicker  5   
   -   5        	  Name$	 datePkr_visitDate��5      $
��Left=�60   $��
51,448,0,0q�6K   $	��Top=�6_   $	��120��6w   $��25��6�   �� 5   
   -   5        
 ��Name$
 comboBx_logInTime��5      $
��Left=�6.   $��22��6I   $	��Top=�6U   $	��145��6m   $��212,448,0,0q�6y   �� 5   
   -   5        $
 comboBx_logOutTime��5      $
��Left=�6/   $��389,448,0,0q�6J   $	��Top=�6_   $	��145��6w   $��22��6�   �� 5   
   -   5        $ txtBx_value��5      $
��Left=�6'   $��23��6B   $��557,447,0,0q�6N   $
 Wrap=�6c   $	��Top=�6w   $��50��6�   $ 1��6�   $ 1��6�   �� 5   
   -   5        $ lbl_visitDate��5      $��Visit Date:��6'   $
��Left=�6=   $��
51,417,0,0q�6X   $	��Top=�6l   $
��Bold"�6�   �� 5   
   -   5        $ 
lbl_timeIn��5      $��Log In Time:��6$   $
��Left=�6;   $��212,417,0,0q�6V   $	��Top=�6k   $
��Bold"�6�   �� 5   
   -   5        $ lbl_timeOut��5      $��Log Out Time:��6%   $
��Left=�6=   $��389,417,0,0q�6X   $	��Top=�6m   $
��Bold"�6�   �� 5   
   -   5        $ 	lbl_value��5      $��Value:��6#   $
��Left=�64   $��557,417,0,0q�6O   $	��Top=�6d   $
��Bold"�6|   �� 5    
   -   5        $ btn_saveVisit��5       $
��Save��6(   $
��Left=�67   $��268,486,0,0q�6R   $	��Top=�6g   $	��150��6   6�   5"      5#      