/*
 * *表格组件
 */
var ctrlGrid = function (ele, opt) {
    this.defaults = {
        id: null,
        //请求url
        url: null,
        //表头格式
        columns: null,
        //是否分页
        pagination: false,
        //页显示
        pagerow: 15,
        //页索引
        pageindex: 1,
        //总页数
        totalpage: 1,
        //  排序字段
        Sidx: null,
        // 默认排序方式,如:asc
        Sord: "asc",
        Swhere: '',
        dataall: null,
        onSelectRow: function () { },
        Serial: false
    };
    this.params = {
        //页显示
        pagerow: this.defaults.pagerow,
        //页索引
        pageindex: this.defaults.pageindex,
        //  排序字段
        Sidx: this.defaults.Sidx,
        // 默认排序方式,如:asc
        Sord: this.defaults.Sord,
        Swhere: this.Swhere
    };
    this.settings = $.extend({}, this.defaults, opt);
};
ctrlGrid.prototype = {
    _id: null,
    _op: null,
    init: function () {
        this._id = this.settings.id;
        _op = this;
        this.create();
        this.bindEvent();
    },
    create: function () {
        //初始化元素
        this.InitializeElement();
        //初始化表头
        this.createTableHead();
        //初始化动态行
        this.createTableBody(1);
        //选择是否分页
        if (this.settings.pagination) this.createTableFoot();
    }, bindEvent: function () {
        //添加上一页事件
        this.registerUpPage();
        //添加下一页事件
        this.registerNextPage();
        //添加首页事件
        this.registerFirstPage();
        //添加最后一页事件
        this.registerlastPage();
        //添加跳转事件
        this.registerSkipPage();
        //添加点击页数事件
        this.registerClickPage();
        //添加排序点击事件
        this.registerClickAsc();
        this.registerClickDesc();
        //添加获取当前选中行
        //this.registerGetRows();
        //全选
        this.registerAllcheck();
        //点击选中
        this.registerSelectRow();
    },
    //初始化元素
    InitializeElement: function () {
        $.authbutton();
        $(this._id).empty().append("<table class='table table-hover'><thead><tr></tr></thead><tbody></tbody></table>");
        $(this._id).addClass("ctrltable");
    },
    createTableHead: function () {
        var cols = this.settings.columns;
        var self = this;
        _.forEach(cols, function (col, i) {
            if (!col.hidden) {
                if (col.field === "ck") {
                    $(self._id + " thead tr").append("<th style='text-align:center;width:50px'><input  type='checkbox' id='allcheck'></th>");
                  
                } else {
                    if (i==0) {
                        if (self.settings.Serial) {
                            $(self._id + " thead tr").append("<th></th>");
                        }
                    }
                    $(self._id + " thead tr").append("<th style='text-align:" + col.align + "; width:" + col.width + "px'  data-name=" + col.field + "><span>" + col.title + "</span>" +
                        (col.sort ? "<span><i class='page-sort-asc'></i><i class='page-sort-desc'></i></span>" : "") +
                        "</th>");
                }
            }
        });//请求数据
    },
    //内容填充
    createTableBody: function (pn) {
       var index= layer.load(1);
        var columns = _op.settings.columns;
        var self = this;
        self.settings.pageindex = pn;
        self.params.pagerow = self.settings.pagerow;
        self.params.pageindex = pn;
        self.params.Sidx = self.settings.Sidx;
        self.params.Sord = self.settings.Sord;
        _.assign(self.params, self.settings.Swhere);
        $post(_op.settings.url, $.param(self.params)).then(function (json) {
            self.settings.dataall=json;
            self.settings.totalpage = json.total;
            var rowsdata = "";
            if (!self.settings.pagination) {
                json.rows = json;
            }
            _.forEach(json.rows, function (data, i) {
                rowsdata += "<tr data-id=" + i + ">";

                _.forEach(columns, function (column, j) {
                    if (j==0) {
                        if (self.settings.Serial) {
                            rowsdata += `<td style='width:10px'>${(i + 1) + (self.params.pagerow * (pn - 1))}</td>`;
                        }
                    }

                    if (!column.hidden) {
                        if (column.field === 'ck') rowsdata += '<td  style="text-align: center;width:50px" ><input name="chk"  type="checkbox"></td>';
                        else {
                            if (column.forrmatter) {
                                rowsdata += '<td  data-field=' + column.field + '  style="text-align: ' + column.align + '; "><div class=pagetable-cell  style="width:' + column.width + 'px;">' + column.forrmatter(json.rows[i], i) + '</div></td>';
                            } else {
                                rowsdata += '<td data-field=' + column.field + '      style="text-align:' + column.align + ';"><div style="width:' + column.width + 'px;" class=pagetable-cell     title=\'' +$.escapeHtml((json.rows[i][column.field] == null ? "" : json.rows[i][column.field]))+'\'>' + (json.rows[i][column.field] == null ? "" : json.rows[i][column.field]) + '</div></td>';
                            }
                        }
                    }
                });
                rowsdata += "</tr>";
            });
            $(self._id + " tbody ").empty().append(rowsdata);
            if (self.settings.pagination) self.createTableFoot();
            layer.close(index);
        });
      
    },
    //初始化分页
    createTableFoot: function () {
        if (typeof(this.settings.totalpage) ==='undefined') {
            return false;
        }
        var footHtml = '          <div class="pull-right"> <nav aria-label="..."> <ul class="pager">';
        //footHtml += "<span id='firstPage'><a disabled='disabled' >首页</a></span>";
        //footHtml += " <span id='UpPage'><a disabled='disabled'>上一页</a></span>";
        footHtml += '<li><a id="UpPage">&laquo;上一页</a></li>';
        var self = this;
        if (self.settings.pageindex <= self.settings.totalpage) {
            for (x = (self.settings.pageindex - 2); x < (self.settings.pageindex + 2); x++) {
                if (x < self.settings.totalpage && x > 0) {
                    if (x == self.settings.pageindex) {
                        footHtml += " <li class='disabled'><a>" + self.settings.pageindex + "</a></li>";
                    } else {
                        footHtml += " <li id='clickPage'><a>" + x + "</a></li>";
                    }
                }
            }
        }
        if ((!(self.settings.totalpage === 1)) && self.settings.totalpage>2) {
            footHtml += " <li><a>...</a></li>";
        }
        if (self.settings.totalpage === self.settings.pageindex) {
            footHtml += " <li  class='disabled'><a>" + self.settings.pageindex + "</a></li>";
        } else {
            footHtml += " <li id='clickPage'><a>" + self.settings.totalpage + "</a></li>";
        }
        footHtml += '<li><a id="nextPage">下一页&raquo;</a></li>';
       // footHtml += " <span id='clickPage'>" + self.settings.totalpage + "</span>";
        //footHtml += "<span id='nextPage'><a>下一页</a></span>";
        //footHtml += "  <span id='lastPage'><a>尾页</a></span>";
        //footHtml += "<input type='text'/><span id='skippage'><a>跳转</a></span>";
        footHtml += " </ul></nav></div>";
        $(this._id + ' .table').next().remove();
        $(this._id + ' .table').after(footHtml);
    },
    /*--------分页事件注册----------*/
    /*========首页事件========*/
    registerFirstPage: function () {
        $(this._id).delegate("#firstPage", "click", function () {
            _op.settings.pageindex = 1;
            _op.createTableBody(_op.settings.pageindex);

        });
    },
    /*==上一页事件==*/
    registerUpPage: function () {
        $(this._id).delegate("#UpPage", "click",
            function () {
                if (_op.settings.pageindex === 1) {
                    $.Alert({ resultSign: 1, message: "已经是第一页" });
                    return;
                }
                _op.settings.pageindex = _op.settings.pageindex - 1;
                _op.createTableBody(_op.settings.pageindex);
            });
    },
    /*添加下一页事件 */
    registerNextPage: function () {
        $(this._id).delegate("#nextPage", "click", function () {
            if (_op.settings.pageindex === _op.settings.totalpage) {
                $.Alert({ resultSign: 1, message: "已经是最后一页了" });
                return;
            }
            _op.settings.pageindex = _op.settings.pageindex + 1;
            _op.createTableBody(_op.settings.pageindex);
        });
    },
    /*添加尾页事件*/
    registerlastPage: function () {
        $(this._id).delegate("#lastPage", "click", function () {
            _op.settings.pageindex = _op.settings.totalpage;
            _op.createTableBody(_op.settings.totalpage);
        });
    },
    /*添加页数跳转事件*/
    registerSkipPage: function () {
        $(this._id).delegate("#skippage", "click", function () {
            var value = $([this._id] + " tfoot tr td input").val();
            if (!isNaN(parseInt(value))) {
                if (parseInt(value) <= _op.settings.totalpage) _op.createTableBody(parseInt(value));
                else $.Alert({ resultSign: 1, message: "超出页总数" });
            } else $.Alert({ resultSign: 1, message: "请输入数字" });
        });
    },
    /*添加点击页数事件 */
    registerClickPage: function () {
        $(this._id).delegate("#clickPage", "click", function () {
            _op.settings.pageindex = parseInt(this.innerText);
            _op.createTableBody(_op.settings.pageindex);
        });
    },
    /*添加排序点击事件*/
    registerClickAsc: function () {
        $("table").delegate(".page-sort-asc", "click", function () {
            $("i").removeAttr("sort");
            $(this).attr("sort", "asc");
            _op.settings.Sidx = $(this).parent().parent().data("name");
            _op.settings.Sord = "ASC";
            _op.createTableBody(1);

        });
    }, registerClickDesc: function () {
        $("table").delegate(".page-sort-desc", "click", function () {
            $("i").removeAttr("sort");
            $(this).attr("sort", "desc");
            _op.settings.Sord = "DESC";
            _op.settings.Sidx = $(this).parent().parent().data("name");
            _op.createTableBody(1);
        });
    }, registersearchClick: function () {
        var params = [];
        $('#toolbar').find('input[name], select').each(function () {
            if ($(this).attr('name')) {
                params[$(this).attr('name')] = $(this).val();
            }
        });
        _op.settings.Swhere = params;
        _op.createTableBody(1);
        return false;
    },//获取选中的行
    registerGetRows: function (callback) {
        var self=this;
        var jsonArr=[];
        var $chkBoxes = $('table').find('input:checked');
        $($chkBoxes).each(function () {
            jsonArr.push(self.settings.dataall.rows[$(this).parent().parent().attr("data-id")]);
        });
        // var jsonArr = [];
        // //遍历被选中的checkbox集
        // $($chkBoxes).each(function () {
        //     var jsonstr = "{";
        //     $('td', $(this).parent().parent()).each(function (index, td) {
        //         if (!($(td).attr("data-field") === undefined)) {
        //             jsonstr += `"${$(td).attr("data-field")}":"${$(td).text()}",`;
        //         }
        //     });
        //     jsonstr = jsonstr.substring(0, jsonstr.length - 1);
        //     jsonstr += "}";
        //     jsonstr = jsonstr.replace(/\n/g, "").replace(/\r/g, "");//去掉字符串中的换行符
        //     jsonstr = jsonstr.replace(/\n/g, "").replace(/\s|\xA0/g, "");//去掉字符串中的所有空格
        //     jsonArr.push(JSON.parse(jsonstr));

        // });
        callback(jsonArr);
    }, registerAllcheck: function () {
        $("table").delegate('#allcheck', "click", function () {
            if (this.checked === true) {
                $("table input[name=chk]").each(function () {
                    this.checked = true;
                });
            } else {
                $("table input[name=chk]").each(function () {
                    this.checked = false;
                });
            }
           
        });
    }, registerSelectRow: function () {
        var self = this;
        $("table").delegate("td", "click", function () {
            $("tr").removeClass("ctrltable-selected");
            obj=self.settings.dataall.rows[$(this).parent().attr("data-id")];
            $(this).parent().addClass("ctrltable-selected");
            self.settings.onSelectRow(obj);
        });
    }, gridRowValue: function () {
        var obj = this.settings.dataall.rows[$(".ctrltable-selected").attr("data-id")];
        return obj;
    }


};
$.fn.ctrlGrid = function (options) {
    var grid = new ctrlGrid(this, options);
    grid.init();
    return grid;
};





