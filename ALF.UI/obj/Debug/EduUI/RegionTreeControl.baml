   M S B A M L   `   `   `  ����A  =ALF.UI, Version=1.0.0.4, Culture=neutral, PublicKeyToken=nullclr-namespace:ALF.UIALF.UI  5      B >ALF.EDU, Version=1.0.1.1, Culture=neutral, PublicKeyToken=nullF0clr-namespace:ALF.EDU.DataModel;assembly=ALF.EDUALF.EDU.DataModel 5      9!clr-namespace:ALF.UI.TitleControlALF.UI.TitleControl  5      $    ALF.UI.EduUI.RegionTreeControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      #uiclr-namespace:ALF.UI     5      F	dataModel0clr-namespace:ALF.EDU.DataModel;assembly=ALF.EDU     5      :titleControl!clr-namespace:ALF.UI.TitleControl     5      .��5      �   � RowDefinitions  5   
   �� 5	      $��40 �6   �� 5
      $��* �6   .+�5   
      ALF.UI.TitleControl.Combo  -   5          Name$ analysisTypeComboBox��5      $��0��6<      Title$ 分析视角��6I      
TitleWidth$ -1Q�6V   R� 5   
   -   5         R�Name$ typeTreeView��5      $��1��6)   
�������$��1q�6J   
������ ��5      �� 5       �� DataType$, &dataModel:return_getRegionTreeNodeList��6   .R�5      ��  �� Orientation$ 
Horizontal=�6"   .+�5      ��  ��Name$ logoRect��6$   -   5        $��20��5   6   $��20��6A   $��5,0,5,0q�6M   
 � Tag 6r   �� *nodeOrganizationType+�� 5      $��Center=�6$   $��12(�6?   ��6M   �� *
nodeName+5      5      5      5      5      