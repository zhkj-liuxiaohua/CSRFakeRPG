# LevelStatusUp
等级属性提升

#### 主要功能
    绑定等级和属性的关联，并提供侧边栏查询属性的功能。目前主要绑定三项基础属性：玩家最大生命值，玩家临时攻击力，玩家抗击飞概率。升级时，将根据配置文件中的设定设置玩家的动态属性。采用xuid对玩家标识符进行记录，需要登录xbox的账号才可应用本插件。

#### 指令说明
/levelstatus

    在侧边栏显示一个状态信息（每秒刷新一次，定时关闭，可在配置文件中修改为常驻显示）
    
    [例] /levelstatus
    
/levelstatus off

    玩家：隐藏侧边栏状态信息
    
    [例] /levelstatus off
    
levelstatus [reload]

    后台：重载配置文件
    
    [例] levelstatus reload
    
levelstatus [edit]

    后台：打开一个配置设定的窗体界面（不适用于面板、无UI系统）
    
    [例] levelstatus edit
