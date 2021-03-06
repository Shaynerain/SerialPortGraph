# SerialPortGraph

显示串口波形，按照指定格式发送，根据收到的数据绘制波形

## 编译环境

- VS2019
- .Net 4.6

## 界面

- 简单易懂

## 波形设置

- 添加波形的名字需要和接收的名字一致
- 颜色可点击颜色按钮选择
- 长度指示波器接收的最大缓存长度

## ~~Json配置（已图形化）~~

* ~~串口名称~~
* ~~波特率~~
* ~~绘制图像的总长度，即x轴长度，和缓存大小有关~~
* ~~波形设置~~
  ~~波形名称~~
  ~~波形颜色~~
  ~~波形放大（未添加）~~
  ~~波形偏移（未添加）~~
  ~~波形显示~~

## 串口发送格式

字符格式发送：

**Name1=Value1; Name2=Value2; \r\n**

*必须包含  '='   ';'*

*Name必须和发送上来的相同*

例子：

一个波形：Line1=0;\r\n

两个波形：Line1=0; Line2=0;\r\n

建议多个指令之间添加延时，测试环境是1ms

可参照此处发送:https://github.com/Shaynerain/STM32-COMP-DEMO

