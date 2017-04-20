# eng-back
作品大赛的后端 
# 目前进度 
开发阶段的用户登录控制已经完成，未来所有API都不需要考虑用户登录控制的问题 
未登录情况下只公开Login和Regist两个接口 其他的自动被中间件屏蔽 
因此往后所有的接口都可以假设用户已经登录

# 整体规划
## 功能说明
此作品为一个英语交流社区，其重点在其社区性质，即其本身不提供内容，而是让注册的用户自己提供内容 
平台本身只提供工具 
用户可以上传文章，导入单词，制作单词包，或者使用提取工具从一篇文章中提取单词和句子，使用Markdown制作单词助记卡片，给单词配音，给句子配语音，单词配朗读，做成“资源包”标价后发布出去，也可以标价为0免费发布出去，每个资源包有自己的使用者和创作者，用户购买后，得到一个资源包的副本（属于用户自己的资源）副本中标明了此资源包为一个副本以及它的创作者还有它的源资源包ID，作者的资源包使用者名单中添加一位使用者，当作者改动资源包时，会通知所有的使用者用户更新此资源包，用户可以选择是否同步，用户还可以将资源包创建一个副本修改后再卖出去，如果用户选择创建副本，则需要对作者发出请求，作者同意并开价，用户支付后，其拥有的资源包的副本标记将移除，所有关于其创作者的标记都会移除，此资源包将作为一个全新的资源包存在，创作者标记为用户本身，一旦用户选择创建副本，则失去跟踪作者更新的权利 
这样就可以建立一个完整的资源交易社区，利润以“收税”形式得到，用户则通过倒卖资源包和充值的方式得到虚拟币，此处有两个方案：
*   第一是虚拟币可以提现，这样就不能允许用户通过完成学习任务奖励等方案获得虚拟币，因为这样提现就得自己出钱，必须保证充值的总额增长总大于奖励分的增长。。这个不现实
*   第二是虚拟币不能提现，而只能用户购买社区内部的资源，这样资源分可以通过奖励给出，同时所有的充值都是盈利 

这里称虚拟币为资源分 
## 模块设计

### 前端模块

#### 学习中心
学习中心主要用于提供所有的学习功能，即对资源包的使用功能，其包括：
*   使用资源包中的单词包背单词（可以选择听机器发音和单词包自带的发音）
*   使用语句包练习听力（类似扇贝听力的那种）和正音（通过后台语音识别进行英语发音识别度测定)
*   使用文章包练习听力，阅读
任何一个单词点击后都能得到翻译和机器发音（预计在未来将可以让用户选择其拥有的资源包中的可用发音，但是这样会大大增加后台数据结构的复杂性，暂时不考虑） 

#### 创作中心
创作中心提供Markdown编辑器（或者富文本编辑器），允许用户上传文章（并自动提取句子和单词），允许用户设置提取工具参数（例如学习历史过滤）， 允许用户上传单词，句子的配音 
上面说的比较模糊，具体是这样的，用户上传一篇文章（也可以是句子集合或者单词集合，单词之间通过空格分割 句子之间通过句点分割，预计未来提供自定义格式化功能），生成一个原始资源包，往后的所有操作都是对此资源包的更改 
用户通过富文本编辑器上传的应该是一个不带Script的html文本，其中的图片的等应通过base64数据或者外部URL引用（利用图床）得到，同时如果存在多媒体内容应通过外部URL引用 
html文本中可以带style标签，可以引用外部样式表等 
当用户需要发布一个资源包时，会经过人工（或者可能机器？）的审核，主要是看其中有没有fixed的position（因为对此html显示的容器会设置为relative，以及overflow auto这样将其中的内容包起来
#### 资源中心
资源中心是资源集中交易的场所，主要提供 资源热度排名 最新排名 分类 作者 等检索功能
#### 广场
类似微博，提供用户社交的简单场所，用户可以在这里进行简单社交
#### 资源卡片页
用户显示一个资源的信息，用户发布自己的资源包时可以选择公开多少信息和哪些信息， 
使用者点开后将进入此页，看到作者公开的信息 
#### 用户中心页
用户显示用户个人的信息，提供例如更改密码，更新个人信息，注销登录，查看自己资源分的功能
#### 用户信息页
用于给其他用户查看某个用户的公开信息
#### 布局页
用于给所有上述页面提供一个整体布局框架，例如一个导航栏等，页脚等
#### 社交小部件
从服务器接受社交信息，例如回复通知等，并提供导航到特定消息页的功能
#### 消息页
用户具体地显示某个社交消息包括其回复等
#### 即时通信组件
这是可能的类似facebook中的那种IM窗口，提供即时通信功能

### 后端整体规划
后端采用，以User为核心，以Username为唯一索引的构造方式，Username为一个字符串 
每个模块有自己的内部接口和外部接口哦 
内部接口直接通过类方法公开，同时对外提供外部接口 
这些内部接口仅能在一个实现内部使用，如果跨不同平台就不行，这里我们约定，对于耦合度高的，即需要使用内部接口才能实现 
的部分，必须在放在同一个平台一起实现 
而对于相关度不大的部分，则可以分到不同平台上

每个模块对外公开自己的接口，对内提供自己的辅助函数和工具函数 
辅助函数为与一个外部调用相关的操作的集合 
工具函数为单纯的处理过程 
除User模块比较特殊外，其他的模块都直接假设用户已经登录 
同时 每个模块可以定义自己的辅助函数和工具函数部分 
可以被外部使用 
每个模块负责一个具体的功能例如用户信息管理 
每个模块有一个自己的DB文件做数据库 
这样做的好处主要是避免出现各个模块的模块名字冲突 
类似命名空间的作用，每个模块的DB文件的名字就是这个模块自己的名字 
这样避免名字冲突
### 后端跨平台说明
对于跨不同平台的，例如php和.net core 还有node.js联合开发 
则需要进行整体约定，这里约定如下
* 不同平台之间只能和前端一样通过各方外部接口进行通信，如果不行，则不能分到两个平台
* 以Username为核心索引
* 除.net core中有特殊的用户用户登录的模块外，其他任何实现中的外部接口都必须严格遵守“未登录不响应”的原则


其他平台实现，可以通过调用User模块中的HasLogin来测试是否登录，通过GetUserInfo接口获取登录的用户信息，包括用户名
