# GitHub Actions 使用指南

## 概述

本项目配置了完整的 GitHub Actions 工作流，用于自动化构建、测试和发布。

## 工作流文件

### 1. `.github/workflows/build.yml`
**基础构建工作流**
- 触发条件：推送到 main、develop、feat/*、hotfix/* 分支或创建 PR
- 功能：编译所有项目并生成构建产物
- 输出：构建产物作为 artifacts 上传

### 2. `.github/workflows/ci-cd.yml`
**完整的 CI/CD 流水线**
- 触发条件：
  - PR：运行测试
  - 推送到 main/develop：构建并生成发布产物
  - 创建版本标签：自动发布到 GitHub Releases
- 功能：测试 → 构建 → 发布
- 输出：测试产物、发布产物、GitHub Release

### 3. `.github/workflows/localization-test.yml`
**本地化测试工作流**
- 触发条件：修改本地化相关文件时
- 功能：验证资源文件、测试本地化服务
- 输出：本地化测试结果

## 使用方法

### 1. 手动触发工作流

1. 进入 GitHub 仓库页面
2. 点击 "Actions" 标签
3. 选择要运行的工作流
4. 点击 "Run workflow" 按钮
5. 选择分支和参数
6. 点击 "Run workflow"

### 2. 通过提交触发

```bash
# 推送到功能分支，触发构建和本地化测试
git push origin feat/my-change

# 推送到主分支，触发完整 CI/CD
git push origin main

# 创建版本标签，触发自动发布
git tag v1.0.0
git push origin v1.0.0
```

### 3. 查看工作流状态

1. 在 GitHub 仓库页面点击 "Actions" 标签
2. 查看工作流运行历史
3. 点击具体运行查看详细日志
4. 下载构建产物（在 "Artifacts" 部分）

## 工作流详解

### 构建工作流 (build.yml)

```yaml
# 触发条件
on:
  push:
    branches: [ main, develop, feat/*, hotfix/* ]
  pull_request:
    branches: [ main, develop ]

# 执行步骤
steps:
  - 检出代码
  - 设置 .NET Framework 4.8
  - 恢复 NuGet 包
  - 编译各个项目
  - 收集构建产物
  - 上传 artifacts
```

### CI/CD 流水线 (ci-cd.yml)

```yaml
# 三个作业
jobs:
  test:     # PR 时运行测试
  build:    # 推送时构建
  release:  # 标签时发布
```

### 本地化测试 (localization-test.yml)

```yaml
# 专门测试本地化功能
- 验证资源文件存在
- 检查资源键匹配
- 编译测试程序
- 上传测试结果
```

## 环境要求

### GitHub Actions 运行环境
- **操作系统**: Windows Latest
- **.NET Framework**: 4.8.x
- **MSBuild**: 内置
- **NuGet**: 内置

### 本地开发环境
- Visual Studio 2019/2022 或 .NET Framework 4.8 SDK
- NuGet 包管理器
- Git

## 常见问题

### 1. 构建失败

**问题**: MSBuild 找不到 .NET Framework 4.8
**解决**: 确保工作流中正确设置了 .NET Framework 版本

```yaml
- name: Setup .NET Framework
  uses: microsoft/setup-dotnet@v3
  with:
    dotnet-version: '4.8.x'
```

### 2. 依赖包恢复失败

**问题**: NuGet 包下载失败
**解决**: 检查网络连接，重试工作流

### 3. 资源文件编译错误

**问题**: 资源文件格式错误
**解决**: 
1. 检查 .resx 文件格式
2. 确保资源键在中文和英文文件中都存在
3. 验证 XML 语法

### 4. 本地化测试失败

**问题**: 本地化服务无法正常工作
**解决**:
1. 检查 `LocalizationServicer.cs` 实现
2. 验证资源文件路径
3. 确认依赖注入配置

## 自定义配置

### 1. 修改触发条件

```yaml
on:
  push:
    branches: [ main, develop, feature/* ]  # 自定义分支
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 0 * * *'  # 每天运行
```

### 2. 添加新的构建步骤

```yaml
- name: Custom step
  run: |
    echo "执行自定义步骤"
    # 你的命令
```

### 3. 修改构建配置

```yaml
env:
  BUILD_CONFIGURATION: Debug  # 改为 Debug
  BUILD_PLATFORM: x64         # 改为 x64
```

### 4. 添加通知

```yaml
- name: Notify on success
  if: success()
  run: |
    echo "构建成功！"
    # 发送通知

- name: Notify on failure
  if: failure()
  run: |
    echo "构建失败！"
    # 发送通知
```

## 最佳实践

### 1. 分支策略
- `main`: 生产代码，触发完整 CI/CD
- `develop`: 开发代码，触发测试和构建
- `feat/*`: 功能分支，触发基础构建
- `hotfix/*`: 热修复，触发快速构建

### 2. 提交信息
使用规范的提交信息格式：
```
feat: 添加新功能
fix: 修复问题
docs: 更新文档
style: 代码格式调整
refactor: 代码重构
test: 添加测试
chore: 构建过程或辅助工具的变动
```

### 3. 版本管理
- 使用语义化版本号：`v1.0.0`
- 创建标签触发自动发布
- 在 Release Notes 中说明变更

### 4. 安全考虑
- 不要在日志中输出敏感信息
- 使用 GitHub Secrets 存储密钥
- 定期更新依赖包

## 监控和维护

### 1. 工作流监控
- 定期检查工作流运行状态
- 关注构建时间和成功率
- 监控资源使用情况

### 2. 依赖更新
- 定期更新 GitHub Actions 版本
- 更新 .NET Framework 版本
- 检查安全漏洞

### 3. 性能优化
- 使用缓存减少构建时间
- 并行执行独立任务
- 优化构建步骤

## 故障排除

### 查看日志
1. 进入 Actions 页面
2. 点击失败的工作流
3. 查看具体步骤的日志
4. 根据错误信息定位问题

### 常见错误
- **MSB3644**: 缺少 .NET Framework 引用程序集
- **NU1101**: NuGet 包源不可用
- **CS0234**: 命名空间不存在
- **XAML 错误**: 资源文件格式问题

### 调试技巧
1. 在本地重现问题
2. 使用详细日志模式
3. 分步执行工作流
4. 检查环境差异

---

更多信息请参考 [GitHub Actions 官方文档](https://docs.github.com/en/actions)。 