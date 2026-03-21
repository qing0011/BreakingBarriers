mergeInto(LibraryManager.library, {
    OpenGameCircle: function () {
        console.log('===== OpenGameCircle =====');

        if (typeof wx === 'undefined') {
            console.error('wx 不存在');
            return;
        }

        if (!wx.createPageManager) {
            console.error('不支持 createPageManager');
            return;
        }

        const pageManager = wx.createPageManager();

        const openlink = '-SSEykJvFV3pORt5kTNpSxu6XMpdVXR-y6Cx3r60YZyzw4q54fmwKhAZpfR1nEMxYSxZYCKp1zuHEjFRV5XrP-Xvzukn_kl80B7QAloWl69ZbVtFHeu6K2zwrITMYhaBPR_GlXArzqCLmb7muP_QAY-K91cZ8djUX9qj7dpFU3d3KaKmqVJ9ddMyqYnrftRuFvW6IpGTM1mWKiYH9WaFiIUeievyPijBweczoL15F97Ndo38foqZHJnEOptUh4ypjQXSofIJpsJHB2w90sEBebSMhb5G4UE89j1dTypgeAdcI6aPwYvQ0js8w2WVWsRffF7BapDvZ1oF8g6XU98iTA';

        pageManager.load({
            openlink: openlink,
            success: function () {
                console.log('load 成功');

                try {
                    pageManager.show();
                    console.log('show 成功');
                } catch (e) {
                    console.error('show 失败', e);
                }
            },
            fail: function (err) {
                console.error('load 失败', err);

                wx.showToast({
                    title: '游戏圈打开失败',
                    icon: 'none'
                });
            }
        });
    }
});