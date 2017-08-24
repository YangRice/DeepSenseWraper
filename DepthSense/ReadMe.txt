DepthSense.sln 是用 Visual Studio 2015 撰寫，由 3 個 project 構成:
(注意: Unity 32 bit 版本要使用 x86 編譯，64 bit 版本要使用 x64 編譯)

1. Coord3D.dll
	原生 c++ 的 Export dll。負責與DepthSense.dll溝通
	定義了基本的資料結構與匯出的函式介面，也包含了 DepthSense 的初始化設定 (例如需要 Export 的資料)
	
	目標檔案產生至:
		x64-
			x64\Debug\Coord3D.dll
			x64\Release\Coord3D.dll
		x86-
			Debug\Coord3D.dll
			Release\Coord3D.dll

	Additional Include Directories: 
		C:\Program Files\SoftKinetic\DepthSenseSDK\include
	Additional Library Directories (以 x64 的作業系統為例):
		x64-
			C:\Program Files\SoftKinetic\DepthSenseSDK\lib
		x86-
			C:\Program Files (x86)\SoftKinetic\DepthSenseSDK\lib
	Additional Dependencies:
		DepthSense.lib;
	
	以下是編譯完成後做的複製指令，直接搬到單元測試資料夾、Unity 專案資料夾
	你應該要修改 Unity 專案資料夾，避免複製失敗
	註: 必須放置於 Unity 專案根目錄
	Post-Build Event:
		copy "$(OutDir)$(TargetFileName)" "$(SolutionDir)DepthSenseWarperTests\bin\$(Configuration)\$(TargetFileName)"
		copy "$(OutDir)$(TargetFileName)" "C:\Users\Public\Documents\Unity Projects\Rice Test Meta\$(TargetFileName)"
		
	(題外話，這個 dll 叫做 Coord3D 是我隨便亂取的，詞不達意，別理他)
	
2. DepthSenseWarper.dll
	C# dll，會載入 Coord3D.dll，負責取得 DepthSense 的資料後，轉換為 C# 的 class
	你也可以不使用這個 dll，直接把 DepthSenseWarper.cs 搬到 Unity 裡面用
	
	目標檔案產生至:
		DepthSenseWarper\bin\Debug\DepthSenseWarper.dll
		DepthSenseWarper\bin\Release\DepthSenseWarper.dll
	
	Target Platform: x64
	
	以下是編譯完成後做的複製指令，直接搬到 Unity 專案資料夾
	你應該要修改 Unity 專案資料夾，避免複製失敗
	註: 必須放置於 Asset 目錄
	Post-Build Event:
		copy "$(TargetPath)" "C:\Users\Public\Documents\Unity Projects\Rice Test Meta\Assets\$(TargetFileName)"
	
3. DeptnSenseWarperTests
	針對 DepthSenseWarper 的單元測試。不直接相關，有興趣可以看
	p.s. 我的 IDE 只允許在 x86 專案下執行單元測試