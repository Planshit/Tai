//  ws心跳间隔
const TEN_SECONDS_MS = 10 * 1000;
//  焦点检测间隔
const FOCUSED_CHECK_MS = 1000;
//  自动重新通知间隔
const RENOTIFY_MS = 10 * 1000;
//  重连失败次数进入睡眠状态
const RECONNECTFAIL_SLEEP = 5;
//  Tai WS服务器URL
const WSURL = 'ws://127.0.0.1:8908/TaiWebSentry';

let webSocket = null;
let isConnected = false;
let isChromeFocused = true;
let autoReConnectIntervalId = null;
//  是否处于睡眠状态（来源Tai通知）
let isSleep = false;
//  焦点网页数据
let activePage = {
    url: '',
    title: '',
    icon: '',
    startTime: '',
    endTime: '',
    duration: 0
};
//  重连失败次数
let reconnectFail = 0;

init();

/**
 * 初始化插件（入口
 */
function init() {
    connect();
    startWatchFocus();
    startRenofity();
}

/**
 * 连接Tai
 */
function connect() {
    webSocket = new WebSocket(WSURL);
    webSocket.onopen = (event) => {
        isConnected = true;
        isSleep = false;
        reconnectFail = 0;
        clearInterval(autoReConnectIntervalId);
        chrome.action.setIcon({ path: 'icons/socket-active.png' });
        keepAlive();
        console.log("已连接Tai!");
    };
    webSocket.onmessage = (event) => {
        console.log(event.data);
        if (event.data === 'sleep') {
            isSleep = true;
            calDuration();
            console.log("睡眠");
        }
        else if (event.data === 'wake') {
            isSleep = false;
            console.log("唤醒");
        }

    };
    webSocket.onclose = (event) => {
        isConnected = false;
        chrome.action.setIcon({ path: 'icons/socket-inactive.png' });
        console.warn('与Tai失去连接...');
        webSocket = null;
        startAutoReConnect();
    };
}

function disconnect() {
    if (isConnected && webSocket) {
        webSocket.close();
        isConnected = false;
    }
}

function startAutoReConnect() {
    clearInterval(autoReConnectIntervalId);
    autoReConnectIntervalId = setInterval(
        () => {
            if (!isConnected) {
                console.log("尝试重新连接Tai...")
                connect();
                reconnectFail++;
                //  达到重连失败阈值时进入睡眠状态
                if (reconnectFail >= RECONNECTFAIL_SLEEP && !isSleep) {
                    isSleep = true;
                }
            }
        },
        TEN_SECONDS_MS
    );
}

/**
 * 保持心跳，防止插件掉线
 */
function keepAlive() {
    const keepAliveIntervalId = setInterval(
        () => {
            if (isConnected && webSocket) {
                console.log('ping');
                webSocket.send('ping');
            } else {
                clearInterval(keepAliveIntervalId);
            }
        },
        // It's important to pick an interval that's shorter than 30s, to
        // avoid that the service worker becomes inactive.
        TEN_SECONDS_MS
    );
}


//  Chrome functions

//  监听Chrome选项卡切换事件
chrome.tabs.onActivated.addListener(async function
    (e) {
    const tab = await getTab(e.tabId);
    onActivePage(tab);
}
);

//  监听Chrome选项卡更新事件
chrome.tabs.onUpdated.addListener(function
    (tabId, changeInfo, tab) {
    if (changeInfo.status == 'complete' && tab.active) {
        onActivePage(tab);
    }
}
);

/**
 * 通过TabId获取Tab
 * @param {Number} tabId TabId
 * @returns Tab
 */
function getTab(tabId) {
    return new Promise((resolve, reject) => {
        chrome.tabs.get(
            tabId,
            function (e) {
                resolve(e);
            }
        )
    });
}

/**
 * 指示当前浏览器是否处于焦点中
 * @returns 焦点中返回true
 */
function isFocused() {
    return new Promise((resolve, reject) => {
        chrome.windows.getCurrent(function (w) {
            resolve(w.focused);
        });
    });

}

/**
 * 获取当前焦点Tab
 * @returns Tab Obj
 */
function getCurrentTab() {
    return new Promise((resolve, reject) => {
        chrome.tabs.query({ active: true }, function (tab) {
            if (tab && tab.length > 0) {
                resolve(tab[0]);
            }
            else {
                reject(tab);
            }
        });
    });
}


//  Tai functions

//  通知失败的集合
let notifyFailList = [];

/**
 * 重新发送通知失败的数据（最早的一条）
 */
function renotify() {
    if (isConnected && webSocket && notifyFailList.length > 0) {
        const item = notifyFailList[0];
        notifyFailList.splice(0, 1);
        notifyTai(item);
    }
}

/**
 * 通知Tai更新数据
 * @param {*} data 统计数据
 */
function notifyTai(data = {
    Url,
    Title,
    Icon,
    Duration,
    ActiveTime
}) {
    console.log("notify", data);
    if (isConnected && webSocket) {
        webSocket.send(JSON.stringify(data));
    }
    else {
        // let item = notifyFailList.find(s => s.Url === data.Url);
        // if (item) {
        //     item.Duration += data.Duration;
        // }
        // else {
        notifyFailList.push(data);
        // }

        console.log("failList", notifyFailList)
    }
}

/**
 * 响应网页焦点切换
 * @param {ChromeTab} tab Tab
 */
function onActivePage(tab) {
    //  睡眠状态不处理
    if (isSleep) return;

    if (activePage && activePage.url) {
        if (activePage.url !== tab.url) {
            //  统计时间
            calDuration();
            setActive(tab);
        }
    }
    else {
        setActive(tab);
    }
}

/**
 * 设置当前焦点网页数据
 * @param {*} tab Tab
 */
function setActive(tab) {
    const { url, title, favIconUrl: icon } = tab;
    if (url) {
        activePage = {
            url, title, icon, startTime: new Date().getTime()
        };
    }
    else {
        activePage = null;
    }
}

/**
 * 计算并通知焦点网页浏览时长
 */
function calDuration() {
    if (activePage && activePage.url) {
        let duration = parseInt((new Date().getTime() - activePage.startTime) / 1000);
        let activeTime = parseInt(activePage.startTime / 1000);
        const { url: Url, title: Title, icon: Icon } = activePage;
        activePage = null;
        notifyTai({ Url, Title, Icon, Duration: duration, ActiveTime: activeTime });
    }
}

/**
 * 启动焦点监听（用于检测用户是否处于浏览器中
 */
function startWatchFocus() {
    console.log("开始监听焦点");
    setInterval(
        async () => {
            const focused = await isFocused();
            if (focused) {
                if (!isChromeFocused) {
                    isChromeFocused = true;
                    //  重置统计
                    const tab = await getCurrentTab();
                    onActivePage(tab);
                    console.warn("重置统计")
                }
            }
            else {
                if (isChromeFocused) {
                    isChromeFocused = false;
                    //  更新时间
                    calDuration();
                    console.warn("更新时间")
                }
            }
        },
        FOCUSED_CHECK_MS
    );
}

/**
 * 启动定时重新发送失败的数据通知
 */
function startRenofity() {
    setInterval(() => {
        renotify();
    }, RENOTIFY_MS);
}