   M S B A M L   `   `   `  ���� A  =UIB_AV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:UIB_test_interfaceUIB_test_interface  #    UIB_test_interface.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     2local clr-namespace:UIB_test_interface       � Title$  AV main��$��651.5��$	��953�� � 
ResizeMode$ NoResize=�.���  � ColumnDefinitions �� $	��99* ��� $
��376* �.+��� $��	Scan file��$��Right=�$	��256��$��
0,32,220,0q�$	��Top=�$	��304��$��1��.��� $��	0,10,-2,0q�.+��� -    ��Name$ upload_textBox��$
��Left=�$��37��$��	10,10,0,0q� �� TextWrapping$
 Wrap=�$��TextBox��$	��Top=�$	��191�� �� 	IsEnabled . {�.  �� -    ��Name$ select_upload_butt��$��Select file��$
��Left=�$��	206,9,0,0q�$	��Top=�$��78���� -   $ Static_scan��$��Static analysis��$
��Left=�$��	10,52,0,0q�$	��Top=�$	��150���� $��Lab 5��$
��Left=�$��79��$��	10,77,0,0q�$	��Top=�$	��274��.��� $��0,0,0,0q�.+��� -   $ lab5_IP_URL1��$��IP URL 1��$
��Left=�$��	10,10,0,0q�$	��Top=�$��75���� -   $ lab5_IP_URL2��$��IP URL 2��$
��Left=�$��	10,35,0,0q�$	��Top=�$��75���� $��Cloud��$
��Left=�$��
10,156,0,0q�$	��274��.��� $��0,0,0,0q�.+��� -   $ 
Ip_addrBox��$
��Left=�$��23��$��0,1,0,0q�$
 Wrap=�$��192.168.31.212��$	��Top=�$��89���� -   $ 
send_cloud��$��Send to Cloud��$
��Left=�$��	156,0,0,0q�$��98��$	��Top=��� -   $ update_button��$��Update��$
��Left=�$��
156,25,0,0q�$	��Top=�$��98���� $��Service status��$
��Left=�$	��256��$��
537,32,0,0q�$	��Top=�$	��195��$��1��.��� $��	0,0,12,-4q�.+��� -	    ��Name$ Dyn_checkBox��$��Dynamic analysis��$
��Left=�$��	10,10,0,0q�$	��Top=�{�.  �� -
   $ Log_checkBox��$��Logging server��$
��Left=�$��	10,30,0,0q�$	��Top=�{�.   J� RenderTransformOrigin$ 
0.99,1.333&��� -   $ StatScan_checkBox��$��Static file checking...��$
��Left=�$��	10,50,0,0q�$	��Top=�$ 
0.99,1.333&�{�.  �� -   $ CloudScan_checkBox��$��Cloud Scan running...��$
��Left=�$��	10,70,0,0q�$	��Top=�$ 
0.99,1.333&�{�.  �� -   $ CloudUpdate_checkBox��$ ��Cloud Update is running...��$
��Left=�$��	10,90,0,0q�$	��Top=�$ 
0.99,1.333&�{�.  �� -   $ File_Filt_checkBox��$��File Filter is running...��$
��Left=�$��
10,110,0,0q�$	��Top=�$ 
0.99,1.333&�{�.  �� -   $ Net_Drv_checkBox��$��Net Filter is running...��$
��Left=�$��
10,130,0,0q�$	��Top=�$ 
0.99,1.333&�{�.  �� -   $ Static_checkBox_Copy��$��Satic drv is running...��$
��Left=�$��
10,150,0,0q�$	��Top=�$ 
0.99,1.333&�{�.  �� $
��Left=�$	��272��$��	10,16,0,0q�$	��Top=�$	��411��$��2��.@��� $��File filter��$��60��.��� 
������� �� $	��32* ��� $
��103* �.+��� $��Protected files��$
��Left=�$	��250��$	��Top=�$	��409��$��	-4,0,0,-6q�$��2��.��� $��
0,0,-12,-1q� �� $	��10* ��� $	��80* ��� $
��309* �.+��� -   	 ��Name$	 	filesList��$��	0,10,10,0q�$	��159��$	��Top=�$��1��$��2���� -   $ add_butt��$��Add new��$��179,174,10,10q�$��2���� -   $ Clear_button��$��
Clear list��$
��Left=�$��
0,174,0,35q�$��75��$��1���� -   $��Refresh��$
��Left=�$��
0,199,0,10q�$��75��$��1���� $��
Reg filter��$��60��.��� 
������� �� $	��86* ��� $
��319* �.+�� $
��Left=�$	��244��$	��Top=�$	��405��$��2���� $��Add reg key��$
��Left=�$��66��$	��Top=�$	��405��$��2��.��� $
��Left=�$��0,-11,-2,-12q�$	��395��.+��� -   $ add_ref_button��$��Add rule��$
��Left=�$��	10,38,0,0q�$	��Top=�$��75���� -   $ reg_path_text��$
��Left=�$��23��$��	10,10,0,0q�$
 Wrap=�$%��\REGISTRY\MACHINE\SOFTWARE\test��$	��Top=�$	��375���� -   $	 reg_list��$
��Left=�$	��163��$��	10,71,0,0q�$	��Top=�$	��385��$��2���� $��	Net rules��$
��Left=�$��20��$	��Top=�$��60��.��� ���	#FFE5E5E5 �� $��2* ��� $	��13* �.+��� -   $ add_net_rule��$��Add rule��$
��Left=�$��266,214,0,0q�$	��Top=�$��75��$��1���� $
��Left=�$��	10,10,0,0q�$
��Wrap=�$��Action:��$	��Top=��� -   $ 	LocIPText��$
��Left=�$��23��$��	10,38,0,0q�$
 Wrap=�$��192.168.195.1��$	��Top=�$	��120��$��1���� $
��Left=�$��	10,39,0,0q�$
��Wrap=�$��Local IP��$	��Top=��� -   $ 	RemIPText��$
��Left=�$��23��$��	38,99,0,0q�$
 Wrap=�$��192.168.195.1��$	��Top=�$	��120��$��1���� $
��Left=�$��
10,100,0,0q�$
��Wrap=�$��	Remote IP��$	��Top=�$��2���� -   $ LocPortText��$
��Left=�$��23��$��
244,39,0,0q�$
 Wrap=�$	��Top=�$��82��$��1���� $
��Left=�$��
174,40,0,0q�$
��Wrap=�$��
Local Port��$	��Top=�$��1���� -   $ RemPortText��$
��Left=�$��23��$��248,100,0,0q�$
 Wrap=�$	��Top=�$��82��$��1���� $
��Left=�$��174,103,0,0q�$
��Wrap=�$��Remote  Port��$	��Top=�$��1���� -   
 ��Name$
 ProtocolBox��$
��Left=�$��
244,10,0,0q�$	��Top=�$��65��$��1��.@��� -   $	��TCP��$
��Left=�$	��118���� -   $	��UDP��$
��Left=�$	��118���� -    $
��ICMP��$
��Left=�$	��118�� �� 
IsSelected . �� $
��Left=�$��
174,13,0,0q�$
��Wrap=�$��Protocol��$	��Top=�$��1���� -!   $ DataPattText��$
��Left=�$��23��$��
30,163,0,0q�$
 Wrap=�$
��data��$	��Top=�$��82��$��1���� $
��Left=�$��
10,166,0,0q�$
��Wrap=�$��Data pattern��$	��Top=�$��2���� -"   $
 	ActionBox��$
��Left=�$��10,7,0,0q�$	��Top=�$��65��$��1��.@��� $	��Log��$
��Left=�$��92���� $��Block��$
��Left=�$��92�� . �� $
��Left=�$��175,163,0,0q�$
��Wrap=�$��	Direction��$	��Top=�$��1���� -#   $
 DirectionBox��$
��Left=�$��229,160,0,0q�$	��Top=�$��65��$��1��.@��� $��In��$
��Left=�$��92�� . �� $	��Out��$
��Left=�$��92���� -$   $ DataOffsetText��$
��Left=�$��23��$��
30,184,0,0q�$
 Wrap=�$��0��$	��Top=�$��82��$��1���� $
��Left=�$��
10,187,0,0q�$
��Wrap=�$��Data offset��$	��Top=�$��2���� $��Drv&toggles��$��80��.��� ���	#FFE5E5E5 �� $��5* ��� $	��22* �.+�� $��2��.+��� $��Dynamic analysis driver ��$
��Left=�$��51��$��	10,10,0,0q�$	��Top=�$	��184��.��� $
��Left=�$��26��$��
10,4,-2,-1q�$	��Top=�$	��164��.+��� -%   $��Start��$
��Left=�$��59��$��20��$��Bottom=�$��0,0,0,6q��� -&   $
��Stop��$
��Left=�$��73,0,0,0q�$	��Top=�$��65���� $��Static analysis driver ��$
��Left=�$��51��$��	10,66,0,0q�$	��Top=�$	��184��.��� $
��Left=�$��26��$��
10,4,-2,-1q�$	��Top=�$	��164��.+��� -'   $ stat_start_button��$��Start��$
��Left=�$��59��$��20��$��Bottom=�$��0,0,0,6q��� -(   $ stat_stop_button��$
��Stop��$
��Left=�$��73,0,0,0q�$	��Top=�$��65���� $��debug
��$
��Left=�$��49��$��
10,189,0,0q�$	��Top=�$	��247��$y�Hidden=�.��� $��
0,-30,-2,3q�$ 0.5,0.5&� �� $	��66* ��� $	��35* �z�V� .~��� �� ��  �� Angle$ -1.011Q�T� .+��� -)   $ fix_links_button��$��	Fix-links��$
��Left=�$��0,18,0,0q�$	��Top=�$��75��$y�Hidden=��� $��Install��$
��Left=�$��67��$��
10,122,0,0q�$	��Top=�$	��196��.��� $��0,0,-2,3q�.+��� -*   $ copy_button��$��Copy_ files��$
��Left=�$��	10,12,0,0q�$	��Top=�$��63���� -+   $��install drv's inf��$
��Left=�$��	78,12,0,0q�$	��Top=�$��98���� $��File Filter driver ��$
��Left=�$��51��$��
211,10,0,0q�$	��Top=�$	��184��.��� $
��Left=�$��26��$��
10,4,-2,-1q�$	��Top=�$	��164��.+��� -,   $ file_start_button1��$��Start��$
��Left=�$��59��$��0,0,0,6q��� --   $ file_stop_button1��$
��Stop��$
��Left=�$��73,0,0,0q�$	��Top=�$��65���� $��Net Filter driver ��$
��Left=�$��81��$��
211,66,0,0q�$	��Top=�$	��184��.��� $
��Left=�$��
10,4,-2,-1q�$	��164��.+��� -.   $ Net_Filter_start_button��$��Start��$
��Left=�$��59��$��20��$	��Top=��� -/   $ Net_Filter__stop_button��$
��Stop��$
��Left=�$��	73,0,0,36q�$��Bottom=�$��65���� -0   $ Net_Filter_restart_button��$��Restart��$
��Left=�$��59��$��	0,26,0,10q��� $
��Left=�$	��318��$��
10,292,0,0q�$	��Top=�$	��920��$ 0.5,0.5&�$��2��z�V� .~��� ��  �� AngleY$ -0.139Q��� T�  T� Y$ -0.498Q�.@��� $	��Log��$��54��.��� ���	#FFE5E5E5 �� $	��70* ��� $��6* ��� $
��637* ��� $
��201* �.+��� -1   $��Refresh��$
��Left=�$��4,10,0,0q�$	��Top=�$��75��$��3���� -2    ��Name$ LogList��$��10,38,10,10q�$��4�� �� View �� .P���  �� Width$	 120��$
��TIME�� �� DisplayMemberBinding �� *TIME+�� $ 50��$	��PID�� �� *PID+�� $ 50��$	��TID�� �� *TID+�� $	 120��$��Function�� �� *
Function+�� $	 400��$��Params�� �� *Params+�� -3   $ start_logging_Copy��$��Start Logging service��$
��Left=�$	��Top=�$	��122��$��
505,10,0,0q�$��2���� -4   $ 	Log_clear��$��Clear��$��3��$
��Left=�$��	95,10,0,0q�$	��Top=�$��75���� $��File&Reg log��$
��Left=�$��22��$	��Top=�$��80��.��� ���	#FFE5E5E5.+��� -5   $ File_log_List��$��
10,35,10,2q� �� .P��� $	 100��$
��Date�� �� *data+�� $	 100��$
��Time�� �� *time+�� $	 650��$��Event�� �� *Event+�� -6   $ file_drv_log_clear��$��Clear Rules��$
��Left=�$��
640,10,0,0q�$	��Top=�$��76���� -7   $ file_drv_log_refresh��$��Refresh��$
��Left=�$��
721,10,0,0q�$	��Top=�$��75���� $��	Net rules��.��� ���	#FFE5E5E5.+�� $
��Left=�$	��268��$��	10,10,0,0q�$	��Top=�$	��894��$��2��.+��� -8   $ NetRuleList��$��10,38,10,10q�$��4�� �� .P��� $ 50��$��Number�� �� *number+�� $ 50��$��Action�� �� *Action+�� $ 50��$��Protocol�� �� *
Protocol+�� $ 60��$��	Direction�� �� *	Direction+�� $	 120��$��LocalIP�� �� *	LocalIP+�� $ 60��$��	LocalPort�� �� *	LocalPort+�� $	 120��$��RemIP�� �� *RemIP+�� $ 60��$��RemPort�� �� *	RemPort+�� $ 70��$��DataPattern�� �� *DataPattern+�� $ 60��$��
DataOffset�� �� *
DataOffset+�� -9   $��Refresh��$
��Left=�$��
774,10,0,0q�$	��Top=�$��75���� -:   $��Clear Rules��$
��Left=�$��
702,20,0,0q�$	��Top=�$��75��$y�Hidden=�