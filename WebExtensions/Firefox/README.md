# Firefox WebExtension 项目

## 项目概述
本目录包含针对 Firefox 浏览器的 WebExtension 实现。源码包括 `manifest.json`、背景脚本 `background.js`、图标等资源。

## 操作系统与构建环境要求
- **操作系统**：Windows 10/11（亦可在 macOS、Linux 上使用相同脚本的 Bash 版本）  
- **Node.js**：版本 ≥ 18（推荐使用 LTS 版本）  
- **npm**：版本 ≥ 9  
- **web-ext**：用于打包、签名和运行扩展的官方工具  

## 所需程序及安装步骤
1. **安装 Node.js 与 npm**  
   前往 https://nodejs.org/ 下载并安装 LTS 版本，安装后 `node -v` 与 `npm -v` 应分别显示 ≥ 18 与 ≥ 9。

2. **全局安装 web-ext（如未安装）**  
   ```powershell
   npm i -g web-ext
   ```
   安装完成后可通过 `web-ext --version` 验证。

## 构建脚本
项目根目录下提供 `build.bat`（Windows）脚本，执行以下全部步骤：

```bat
@echo off
setlocal

rem ---- 1. 检查 Node 与 npm 版本 ----
for /f "tokens=*" %%i in ('node -v') do set NODE_VER=%%i
for /f "tokens=*" %%i in ('npm -v') do set NPM_VER=%%i

echo Detected Node version: %NODE_VER%
echo Detected npm version: %NPM_VER%

rem 需要的最低版本
set MIN_NODE=v18.0.0
set MIN_NPM=9.0.0

rem 简单的版本比较（仅检查主版本号）
for /f "tokens=2 delims=v." %%i in ("%NODE_VER%") do set NODE_MAJOR=%%i
for /f "tokens=1 delims=." %%i in ("%MIN_NODE%") do set MIN_NODE_MAJOR=%%i

for /f "tokens=1 delims=." %%i in ("%NPM_VER%") do set NPM_MAJOR=%%i
for /f "tokens=1 delims=." %%i in ("%MIN_NPM%") do set MIN_NPM_MAJOR=%%i

if %NODE_MAJOR% LSS %MIN_NODE_MAJOR% (
    echo Error: Node version must be >= %MIN_NODE%
    exit /b 1
)

if %NPM_MAJOR% LSS %MIN_NPM_MAJOR% (
    echo Error: npm version must be >= %MIN_NPM%
    exit /b 1
)

rem ---- 2. 安装 web-ext（如果未全局安装）----
where web-ext >nul 2>&1
if errorlevel 1 (
    echo web-ext not found, installing globally...
    npm i -g web-ext
    if errorlevel 1 (
        echo Failed to install web-ext.
        exit /b 1
    )
) else (
    echo web-ext already installed.
)

rem ---- 3. 执行构建 ----
rem 创建输出目录
if not exist dist mkdir dist

echo Building Firefox extension...
web-ext build --source-dir . --artifacts-dir ./dist
if errorlevel 1 (
    echo Build failed.
    exit /b 1
)

echo Build succeeded. Artifact saved in ./dist
endlocal
```

> **使用方法**  
> 1. 将 `build.bat` 放置于 `Firefox` 目录根部。  
> 2. 在 PowerShell 或 CMD 中切换到该目录：`cd Firefox`。  
> 3. 运行脚本：`.\build.bat`。  
> 4. 成功后，`dist` 目录下会出现类似 `my-extension-0.0.1.xpi` 的文件，即可在 Firefox 中加载测试。

## 常见问题
- **找不到 web-ext**：确保全局 `npm` 路径已加入系统 `PATH`，或手动执行 `npm i -g web-ext`。  
- **Node 版本过低**：请升级至 LTS 版本（≥ 18），或使用 nvm 管理多个版本。  
- **构建失败**：检查 `manifest.json` 是否符合 Firefox WebExtension 规范，尤其是 `manifest_version` 与权限声明。

---

## 任务进度
- [x] 查看 Firefox 目录结构  
- [x] 编写 README.md（包含构建步骤、环境要求、所需程序及版本）  
- [x] 编写 build.bat 脚本，实现完整的构建流程  
- [ ] 验证脚本在当前环境下可执行（如需，可自行运行 `Firefox\build.bat`）