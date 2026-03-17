mergeInto(LibraryManager.library, {
    OpenGameCircle: function () {
        console.log('===== JavaScript: OpenGameCircle 被调用 =====');
        
        try {
            if (typeof wx === 'undefined') {
                console.error('微信 API 不存在');
                return;
            }
            
            console.log('微信版本:', wx.version);
            
            if (!wx.createPageManager) {
                console.error('当前微信版本不支持 createPageManager');
                return;
            }
            
            console.log('开始创建 pageManager...');
            var pageManager = wx.createPageManager();
            
            if (!pageManager) {
                console.error('创建 pageManager 返回 null');
                return;
            }
            
            console.log('pageManager 创建成功');
            
            var openlink = '-SSEykJvFV3pORt5kTNpSxu6XMpdVXR-y6Cx3r60YZyzw4q54fmwKhAZpfR1nEMxYSxZYCKp1zuHEjFRV5XrP-Xvzukn_kl80B7QAloWl69ZbVtFHeu6K2zwrITMYhaBPR_GlXArzqCLmb7muP_QAY-K91cZ8djUX9qj7dpFU3d3KaKmqVJ9ddMyqYnrftRuFvW6IpGTM1mWKiYH9WaFiIUeievyPijBweczoL15F97Ndo38foqZHJnEOptUh4ypjQXSofIJpsJHB2w90sEBebSMhb5G4UE89j1dTypgeAdcI6aPwYvQ0js8w2WVWsRffF7BapDvZ1oF8g6XU98iTA';
            
            console.log('开始加载游戏圈...');
            
            pageManager.load({
                openlink: openlink,
                success: function(res) {
                    console.log('========== 游戏圈加载成功 ==========');
                    console.log('返回结果:', res);
                    
                    // 立即显示游戏圈
                    setTimeout(function() {
                        try {
                            console.log('尝试显示游戏圈...');
                            pageManager.show();
                            console.log('游戏圈显示成功！');
                            
                            // 可选：震动反馈
                            if (wx.vibrateShort) {
                                wx.vibrateShort({
                                    type: 'light'
                                });
                            }
                        } catch (showErr) {
                            console.error('显示游戏圈失败:', showErr);
                        }
                    }, 500); // 增加延迟到500ms
                },
                fail: function(err) {
                    console.log('========== 游戏圈加载失败 ==========');
                    console.log('错误详情:', err);
                    
                    // 显示错误提示
                    if (wx.showToast) {
                        wx.showToast({
                            title: '打开游戏圈失败',
                            icon: 'none',
                            duration: 2000
                        });
                    }
                }
            });
        } catch (e) {
            console.error('========== 打开游戏圈异常 ==========');
            console.error('异常信息:', e);
        }
    }
});