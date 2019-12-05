//闭包限定命名空间
(function ($) {
    if (!$) {
        throw new TypeError("jQuery库不可使用!");
    }
    $.extend({
        Alert: function (Data,callback) {
            var type = 'error';
            switch (Data.resultSign) {
                case 0:
                    type = 'success';
                    break;
                case 1:
                    type = 'warning';
                    break;
                case 2:
                    type = 'error';
                    break;
            }
            const Toast = Swal.mixin({
                toast: true,
                showConfirmButton: false,
                timer: 3000
            });

            Toast.fire({
                type: type,
                title: Data.message
            }).then(function (value) {
                callback && callback();
            });
       
        
        },
        windowsOpen: function (url, title, width, height) {
            layer.open({
                title: title,
                type: 2,
                maxmin: true,    
                //skin: 'layui-layer-rim', //加上边框
                area: [width, height],
                content: url
            });

        }

    });

})(jQuery);




