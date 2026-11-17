# Windows Beta 构建

- 版本 `0.2.0-beta.1`，Unity `6000.3.19f1`，目标 Windows x64。
- 启用场景按 `LevelSelect → Level01 → Level02` 排列。
- 使用项目中配置的脚本后端，关闭 Development Build、Script Debugging 和 Autoconnect Profiler。

从待发布提交导出独立工程。在 Unity 中选择 **Release → Build Windows Beta**，或用批处理执行 `WindowsBetaBuild.Run`。批处理参数 `-releaseOutput` 指定空产物目录，`-releaseRecordDir` 指定构建记录目录，`-sourceCommit` 指定导出的完整 Git 提交号。

构建入口会写入 `build-report.json` 和包内 `BUILD_INFO.txt`，并复制运行说明、发布说明和许可文件。将整个产物目录压缩为 `ImmuneDefense2D-0.2.0-beta.1-Windows-x64.zip`，再从 ZIP 解压到新目录验收实际 EXE。记录 ZIP 的 SHA-256 及自动检查、人工测试的各自结果。

Alpha 构建入口及旧产物仍保留。需要重建 Alpha 时，请检出对应 Alpha 版本提交。
