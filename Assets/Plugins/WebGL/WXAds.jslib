mergeInto(LibraryManager.library, {

    WX_ShowRewardAd: function (typePtr) {

        var type = UTF8ToString(typePtr);

        if (typeof WX_ShowRewardAd === "function") {
            WX_ShowRewardAd(type);
        }
    }

});