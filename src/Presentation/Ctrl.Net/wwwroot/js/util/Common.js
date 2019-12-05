(function ($) {
    $.fn.serializeJson = function () {
        var serializeObj = {};
        _.forEach(this.serializeArray(), function (obj) {
            serializeObj[obj.name] = obj.value;
        });
        return serializeObj;
    },

        $.fn.isEmptyObject = function (e) {
            var t;
            for (t in e)
                return !1;
            return !0;
        },
        $.fn.ctrldaterangepicker = function () {
            //先初始化
            $(this).val(`${moment().format('YYYY-MM-DD 00:00:00')} 至 ${moment().format('YYYY-MM-DD HH:mm:ss')}`);
            $(this).parent().append('  <input type="hidden" id="startTime" name="startTime" class="form-control" /> <input type="hidden" id="endTime" name="endTime" class="form-control" />');
            $("#startTime").val(moment().format('YYYY-MM-DD 00:00:00'));
            $("#endTime").val(moment().format('YYYY-MM-DD HH:mm:ss'));
            $(this).daterangepicker(
                {
                    showDropdowns: true, //年月份下拉框
                    timePicker: true, //显示时间
                    timePicker24Hour: true, //时间制
                    timePickerSeconds: true, //时间显示到秒
                    //autoApply: true,
                    autoUpdateInput: false,
                    startDate: moment().hours(0).minutes(0).seconds(0), //设置开始日期
                    endDate: moment(new Date()), //设置结束器日期
                    //alwaysShowCalendars: true,
                    ranges: {
                        '今天': [moment(), moment()],
                        '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                        '近7天': [moment().subtract(7, 'days'), moment()],
                        '这个月': [moment().startOf('month'), moment().endOf('month')],
                        '上个月': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                    },
                    locale: {
                        format: "YYYY/MM/DD HH:MM:SS",
                        separator: " - ",
                        applyLabel: "确认",
                        cancelLabel: "清空",
                        fromLabel: "开始时间",
                        toLabel: "结束时间",
                        customRangeLabel: "自定义",
                        daysOfWeek: ["日", "一", "二", "三", "四", "五", "六"],
                        monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"]
                    }
                }
            ).on('cancel.daterangepicker', function (ev, picker) {
                $(this).val("请选择日期范围");
                $("#startTime").val("");
                $("#endTime").val("");
            }).on('apply.daterangepicker', function (ev, picker) {
                $("#startTime").val(picker.startDate.format('YYYY-MM-DD HH:mm:ss'));
                $("#endTime").val(picker.endDate.format('YYYY-MM-DD HH:mm:ss'));
                $("#date2").val(picker.startDate.format('YYYY-MM-DD HH:mm:ss') + " 至 " + picker.endDate.format('YYYY-MM-DD HH:mm:ss'));
            });
        },
        $.fn.ctrldatepicker = function () {
            $(this).datepicker({
                orientation: "right",
                format: 'yyyy-mm-dd',
                language: 'zh-CN',
                todayBtn: true,
                todayHighlight: true,
                autoclose: true,
                viewDate: new Date()
            });

        },
        $.fn.fileimage = function (url, ipt, path) {
            path = path == '' ? null : path;
            //初始化上传控件的样式
            $(this).fileinput({
                language: 'zh', //设置语言
                uploadUrl: url, //上传的地址
                autoReplace: true,
                allowedFileExtensions: ['jpg', 'png', 'gif'],//接收的文件后缀
                removeFromPreviewOnError: true,
                uploadAsync: true, //默认异步上传
                showUpload: false, //是否显示上传按钮
                showRemove: true, //显示移除按钮
                showPreview: true,
                showCaption: true,//是否显示标题
                dropZoneEnabled: false,         //是否显示拖拽区域
                overwriteInitial: false,
                browseClass: "btn btn-primary", //按钮样式  
                minFileCount: 1,
                maxFilesNum: 1,
                showUploadedThumbs: false,//下一批选择上传的文件将从预览中清除这些已经上传的文件缩略图。
                initialPreviewAsData: true,
                maxFileCount: 1, //表示允许同时上传的最大文件个数
                enctype: 'multipart/form-data',
                validateInitialCount: true,
                // initialPreviewShowDelete: false,
                initialPreview: [
                    path
                ],

                previewFileIcon: "<i class='glyphicon glyphicon-king'></i>",
                msgFilesTooMany: "选择上传的文件数量({n}) 超过允许的最大数值{m}！",
                uploadExtraData: function (previewId, index) {   //额外参数的关键点

                }
            }).on('filebatchselected', function (event, files) {//选中文件事件
                $(this).fileinput("upload");
                //$(".fileinput-remove-button").trigger("click");

            }).on("fileuploaded", function (event, data, previewId, index) {
                $(ipt).val(data.response.filepath);
            });


        },
        /**
        * 封装Select2方法请求一次数据
        * @param url
        * @param data
        */
        $.fn.CtrlSelect2 = function (url, data = {},callback) {
            var _self = this;
            $fetch(url, data).then(function (data) {
                $(_self).select2({
                    data: data,
                    placeholder: '请选择',
                    language: "zh-CN",
                    allowClear: true
                });
                callback && callback();
            });
        },
        $.fn.zTreeSelect = function (treeDivId, inputId, treeDataList, onClick) {
            var setting = {
                view: {
                    dblClickExpand: false
                },
                data: {
                    simpleData: {
                        enable: true
                    }
                },
                callback: {
                    // beforeClick: beforeClick,
                    onClick: onClick
                }
            };
            $(inputId).click(function () {
                var cityObj = $(inputId);
                var cityOffset = $(inputId).offset();
                $(treeDivId).css({
                    left: cityOffset.left + "px",
                    top: cityOffset.top + cityObj.outerHeight() + "px"
                }).slideDown("fast");
                $("body").bind("mousedown", onBodyDown);
            });

            function hideMenu() {
                $(treeDivId).fadeOut("fast");
                $("body").unbind("mousedown", onBodyDown);
            }
            function onBodyDown(event) {
                if (!(event.target.id == "menuBtn" || event.target.id == "menuContent" || $(event.target).parents(treeDivId).length > 0)) {
                    hideMenu();
                }
            }
            $.fn.zTree.init(this, setting, treeDataList);
        };
})(jQuery);
jQuery.extend({
    IsEmptyGuid: function (e) {
        if (e === "00000000-0000-0000-0000-000000000000") {
            return true;
        } else {
            return false;
        }
    },
    getQueryString: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    },
    authbutton: function () {
        var menubutton = sessionStorage.getItem('menubutton');
        if (menubutton === null || typeof (menubutton) === "undefined") {
            $post("/sysManage/Permission/GetRoutersByUserId", null).then(function (data) {
                sessionStorage.setItem('menubutton', JSON.stringify(data));
                $.cookie('menubutton', JSON.stringify(data), { expires: 7 });
                data = data.filter(item => item.area.toLowerCase() === location.pathname.split('/')[1].toLowerCase() && item.controller.toLowerCase()
                    === location.pathname.split('/')[2].toLowerCase() && item.action.toLowerCase() === location.pathname.split('/')[3].toLowerCase());
                _.forEach(data, function (item, i) {
                    $("#toolbar .form-inline").append(`<button class="btn btn-default btn-xs" onclick='${item.script}' type="submit"><i class="${item.icon}"></i> ${item.name} </button>`);
                });
            });
        }
        if (menubutton !== null && typeof (menubutton) !== "undefined") {
            var arr = JSON.parse(menubutton);
            arr = arr.filter(item => item.area.toLowerCase() === location.pathname.split('/')[1].toLowerCase() && item.controller.toLowerCase()
                === location.pathname.split('/')[2].toLowerCase() && item.action.toLowerCase() === location.pathname.split('/')[3].toLowerCase());
        }
        _.forEach(arr, function (item, i) {
            $("#toolbar .form-inline").append(`<button class="btn btn-default btn-xs" onclick='${item.script}'   type="submit"><i class="${item.icon}"></i> ${item.name} </button>`);
        });
    },
    editor: function (name) {
        CKEDITOR.replace(name, {
            filebrowserImageUploadUrl: '/sysManage/UserControl/FileSave'

        });
    },
    subeditor: function () {
        for (instance in CKEDITOR.instances)
            CKEDITOR.instances[instance].updateElement();
        return true;
    },
    escapeHtml: function (string) {
        var entityMap = {
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': '&#34;',
            "'": '&#39;',
            "\\": '&#92;',
            "/": '&#x2F;'
        };
        return String(string).replace(/[&<>\"\'\/]/g, function (s) {
            return entityMap[s];
        });
    }

});
