# DouyinCap

借助 PuppeteerSharp 无头浏览器实现的抖音直播弹幕爬虫, 支持使用 HTTP 向其他程序发送弹幕数据

## 使用

程序至少需要传入一个整数作为直播间id, 例如一个直播间的地址是 https://live.douyin.com/114514, 那么直播间 id 是 114514.

```bash
DouyinCap 114514  # 使用这条指令来启动程序开始抓取 114514 直播间的弹幕数据.
```

如果需要显示浏览器, 则传入 --show-browser
```bash
DouyinCap --show-browser 114514
```

## 数据发送

向程序传入一个 HTTP 地址, 程序会将弹幕数据通过 POST 的形式发送到指定地址

```bash
# 使用这条指令来抓取 114514 直播间数据, 并将每一条直播间数据发送到 http://127.0.0.1:1145
DouyinCap --post-addr http://127.0.0.1:1145 114514
```

POST 数据的结构:
```json
{
  "Name": "发送者用户名",
  "Value": "弹幕内容",
}
```

## 已知问题

1. 如果
