# 本地构建

Unity 6000.3.19f1，Windows x86_64，入口仅 Assets/Scenes/Level01.unity。Player 版本 0.1.0-alpha.1。保持现有分辨率、显示模式、渲染管线和脚本后端；BuildOptions.None，关闭 Development、Script Debugging 和 Autoconnect Profiler。

菜单：Release > Build Windows Alpha。批处理入口：WindowsAlphaBuild.Run。可通过 -releaseOutput、-releaseRecordDir、-sourceCommit 指定空输出目录、构建记录目录和来源提交。输出目录非空时拒绝覆盖。

发布时从已提交 Git 基线导出独立工程，再用该工程构建；build-report.json 记录来源提交、Unity、结果、大小、选项与错误。构建完成后创建本地版本标签。ZIP 包含整个构建目录，不包含工程和日志。

字体核对：两个 Noto SDF 均为 Dynamic，源字体、1024 atlas、padding 9、多 atlas 及构建时清空动态数据设置未变；本次不提交工作区动态 atlas/glyph 缓存，保留用户原文件。源 OTF 和字体 LICENSE 已受版本控制。

验收口径：用户已完成 Editor 全流程；独立包只执行实际记录的启动/交互冒烟测试，不能替代完整通关。
