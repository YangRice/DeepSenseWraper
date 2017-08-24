DepthSense.sln �O�� Visual Studio 2015 ���g�A�� 3 �� project �c��:
(�`�N: Unity 32 bit �����n�ϥ� x86 �sĶ�A64 bit �����n�ϥ� x64 �sĶ)

1. Coord3D.dll
	��� c++ �� Export dll�C�t�d�PDepthSense.dll���q
	�w�q�F�򥻪���Ƶ��c�P�ץX���禡�����A�]�]�t�F DepthSense ����l�Ƴ]�w (�Ҧp�ݭn Export �����)
	
	�ؼ��ɮײ��ͦ�:
		x64-
			x64\Debug\Coord3D.dll
			x64\Release\Coord3D.dll
		x86-
			Debug\Coord3D.dll
			Release\Coord3D.dll

	Additional Include Directories: 
		C:\Program Files\SoftKinetic\DepthSenseSDK\include
	Additional Library Directories (�H x64 ���@�~�t�ά���):
		x64-
			C:\Program Files\SoftKinetic\DepthSenseSDK\lib
		x86-
			C:\Program Files (x86)\SoftKinetic\DepthSenseSDK\lib
	Additional Dependencies:
		DepthSense.lib;
	
	�H�U�O�sĶ�����ᰵ���ƻs���O�A�����h��椸���ո�Ƨ��BUnity �M�׸�Ƨ�
	�A���ӭn�ק� Unity �M�׸�Ƨ��A�קK�ƻs����
	��: ������m�� Unity �M�׮ڥؿ�
	Post-Build Event:
		copy "$(OutDir)$(TargetFileName)" "$(SolutionDir)DepthSenseWarperTests\bin\$(Configuration)\$(TargetFileName)"
		copy "$(OutDir)$(TargetFileName)" "C:\Users\Public\Documents\Unity Projects\Rice Test Meta\$(TargetFileName)"
		
	(�D�~�ܡA�o�� dll �s�� Coord3D �O���H�K�è����A�����F�N�A�O�z�L)
	
2. DepthSenseWarper.dll
	C# dll�A�|���J Coord3D.dll�A�t�d���o DepthSense ����ƫ�A�ഫ�� C# �� class
	�A�]�i�H���ϥγo�� dll�A������ DepthSenseWarper.cs �h�� Unity �̭���
	
	�ؼ��ɮײ��ͦ�:
		DepthSenseWarper\bin\Debug\DepthSenseWarper.dll
		DepthSenseWarper\bin\Release\DepthSenseWarper.dll
	
	Target Platform: x64
	
	�H�U�O�sĶ�����ᰵ���ƻs���O�A�����h�� Unity �M�׸�Ƨ�
	�A���ӭn�ק� Unity �M�׸�Ƨ��A�קK�ƻs����
	��: ������m�� Asset �ؿ�
	Post-Build Event:
		copy "$(TargetPath)" "C:\Users\Public\Documents\Unity Projects\Rice Test Meta\Assets\$(TargetFileName)"
	
3. DeptnSenseWarperTests
	�w�� DepthSenseWarper ���椸���աC�����������A������i�H��
	p.s. �ڪ� IDE �u���\�b x86 �M�פU����椸����