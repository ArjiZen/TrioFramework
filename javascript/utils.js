define('utils', ['jquery', 'template', 'bootstrap'], function ($, template) {
    // 新版本的jquery插件工具类，逐步替代jquery.extend.js
    var self = {
        showtime_success: 3000,
        showtime_error: 0,
        /*显示错误信息*/
        showError: function (elementSelector, message, showtime) {
            var that = $(elementSelector);
            that.find('.msg-container').remove();
            var msgContainer = '<div class="msg-container"><div class="alert alert-error"><a href="javascript:void(0);" class="close">×</a>' + message + '</div></div>';
            that.append(msgContainer);

            that.find(".msg-container").animate({ "right": "0" }, 'normal', function () {
                if (showtime) {
                    if (showtime == 0) {
                        return;
                    }
                    setTimeout(function () {
                        self.clear(elementSelector);
                    }, showtime);
                }
            });
            that.on('click', 'a', function () {
                var parent = $(this).parents(".msg-container");
                parent.animate({ "right": "-396px" }, 'normal', function () {
                    parent.hide();
                });
            });
        },
        /*显示操作成功信息*/
        showSuccess: function (elementSelector, message, showtime) {
            var that = $(elementSelector);
            that.find('.msg-container').remove();
            var msgContainer = '<div class="msg-container"><div class="alert alert-success">' + message + '</div></div>';
            that.append(msgContainer);

            that.find(".msg-container").animate({ "right": "0" }, 'normal', function () {
                if (showtime) {
                    if (showtime == 0) {
                        return;
                    }
                    setTimeout(function () {
                        self.clear(elementSelector);
                    }, showtime);
                }
            });
            that.on('click', '.msg-container', function () {
                var width = $(this).css("width");
                $(this).animate({ "right": "-" + width }, 'normal', function () {
                    $(this).hide();
                });
            });
        },
        /*清除成功、错误信息*/
        clear: function (elementSelector) {
            var msgContainer = $(elementSelector).find('.msg-container');
            var width = $(msgContainer).css("width");
            msgContainer.animate({ "right": "-" + width }, 'slow', function () {
                $(msgContainer).remove();
            });
        },
        /*模板绑定*/
        template: function (selector, templateid, data) {
            $(selector).html(template(templateid, data));
        },
        /*设置form表单的值*/
        setValue: function (element, value) {
            var el = $(element);
            if (el.is(":input[type='text']")) {
                // textbox
                el.val(value);
            } else if (el.is(":input[type=checkbox]")) {
                // checkbox
                el.attr('checked', (value == "true" || value == "True" || value == true));
            } else if (el.is("select")) {
                // select
                el.find('option').removeAttr('selected');
                el.find('option[value="' + value + '"]').attr('selected', 'selected');
            }
        }
    };

    // $("xxx").func()
    $.fn.extend({
        /*
         * 说明：点击Panel标题展开收缩Panel内容
         * 用法：
         * $(".panel-container").panelToggle();
         */
        panelToggle: function () {
            $.each($(this), function (i, n) {
                $(n).on('click', ".title > label", function () {
                    $(n).find(".content").slideToggle();
                });
            });
        },
        /*
         * 说明：将下拉列表的每次选中的值绑定到对应的隐藏域中，
         *       隐藏域的name为select的name + 'Text'，例如：select的name为type，那么隐藏域的name为typeText;
         * 用法：
         * $("select").dropdownTextBinder();
         * $("select").dropdownTextBinder('Str'); // 指定隐藏域的name后缀
         */
        dropdownTextBinder: function () {
            var defaultSuffix = "Text";
            if (arguments.length == 1) {
                // 指定后缀
                defaultSuffix = arguments[0];
            }
            $.each($(this), function (i, item) {
                $(item).on('change', function () {
                    var name = $(item).attr('name');
                    var textName = name + defaultSuffix;
                    var selector = $("input[type='hidden'][name='" + textName + "']");
                    if (selector.length == 0) {
                        return;
                    }
                    var option = $(item).find("option:selected");
                    selector.val(option.text());
                });
                $(item).trigger('change');
            });
            return this;
        },
        /*
         * 说明：绑定Div中的分页控件事件，及form中的分页参数字段
         * 用法：
         * $("#list-container").pagerBinder("#form-search");
         */
        pagerBinder: function (formSelector) {
            var len = $(this).length;
            if (len == 0) {
                console.error("pagerBinder:未找到分页控件所在的容器");
                return this;
            }
            if (len > 1) {
                console.error("pagerBinder:分页控件绑定不支持多个容器");
                return this;
            }
            /*添加页码隐藏域文本框*/
            $(formSelector).append("<input type='hidden' name='Page'/>");
            /*绑定分页事件*/
            $(this).on('click', '.list-pager a[data-pager]', function () {
                var enabled = $(this).attr("data-enabled");
                if (enabled == "false") {
                    return;
                }
                var page = $(this).attr('data-pager');
                $(formSelector).find("input[name='Page']").val(page);
                $(formSelector).trigger('submit');
            });
            return this;
        },
        /*
         * 说明：将数据绑定到对应的模板id，默认增加判断是否有数据的参数
         * 用法：
         * $("#list-container").tableBinder('tpl_name', data);
         */
        tableBinder: function (templateid, data) {
            data['empty'] = (data.Option.Total == 0);
            self.template(this, templateid, data);
            return this;
        },
        /*
         * 说明：将数据绑定到对应的模板id
         * 用法：
         * $("#list-container").templateBinder('tpl_name', data);
         */
        templateBinder: function (templateid, data) {
            self.template(this, templateid, data);
            return this;
        },
        /*
         * 说明：使用当前form的action和其中的字段发送post请求
         * 用法：
         * $("#form-search").request(function(){}, function(){});
         */
        request: function (successCallback, failureCallback) {
            var action = $(this).attr('action');
            var parameters = $(this).serialize();
            $.request(action, parameters, successCallback, failureCallback);
            return this;
        },
        /*
         * 说明：在当前元素中弹出提示文本
         * 用法：
         * $("body").alert(true, "操作成功")
         * $("body").alert(false, "操作失败")
         */
        alert: function (ajaxResult, message) {
            if (ajaxResult === true) {
                self.showSuccess($(this), message, self.showtime_success);
                return this;
            }
            if (ajaxResult === false) {
                self.showError($(this), message, self.showtime_error);
                return this;
            }
            //if (!ajaxResult.success) {
            //    self.showError($(this), ajaxResult.errorMessage);
            //} else {
            //    if (message && $.trim(message) != '') {
            //        self.showSuccess($(this), message, self.showtime_success);
            //    }
            //}
            return this;
        },
        /* 
         * 说明：使用json对象为表单赋值
         * 用法：
         * $("form").set({'id': 'abc', 'name': 'myName', 'title': 'myTitle'})
         */
        set: function (option) {
            if (!$(this).is("form")) {
                console.error('set方法只对form表单有效');
                return;
            }

            var leftOption = option;
            var elements = $(this).find(":input");
            for (var i = 0; i < elements.length; i++) {
                var element = $(elements[i]);
                var name = element.attr('name');
                for (var item in leftOption) {
                    if (eval('/' + item + '/ig').test(name)) {
                        self.setValue(element, leftOption[item]);
                        delete leftOption[item];
                        continue;
                    }
                }
            }
        }
    });

    $.extend({
        /*
         * 说明：发送ajax post请求
         * 用法：
         * $.request('class/method', {id: 'xxx'}, function(){}, function(){})
         */
        request: function (url, parameters, successCallback, failureCallback) {
            $.ajax({
                type: 'POST',
                data: parameters,
                dataType: 'json',
                url: url,
                success: function (result) {
                    if (result.success) {
                        if (successCallback) {
                            var data = $.json(result.data);
                            successCallback(result, data);
                        }
                    } else {
                        if (failureCallback) {
                            failureCallback(result);
                        } else {
                            $.alert(result.success, result.errorMessage);
                        }

                    }
                }
            });
        },
        /*
         * 说明：将字符串转为json对象
         * 用法：
         * $.json('{id: "xxx"}')
         */
        json: function (str) {
            if (typeof str == "object") {
                return str;
            }
            if (str == undefined || str == "") {
                return "";
            }
            var decodeStr = str.replace(/\&quot;/g, '"').replace(/&#39;/g, '"');
            if (typeof (JSON) == 'undefined') {
                return eval('(' + decodeStr + ')');
            } else {
                return JSON.parse(decodeStr);
            }
        },
        /*
         * 说明：在body元素中弹出提示文本
         * 用法：
         * $("body").alert(true, "操作成功")
         * $("body").alert(false, "操作失败")
         */
        alert: function (ajaxResult, message) {
            if (ajaxResult === true) {
                self.showSuccess($("body"), message, self.showtime_success);
                return;
            }
            if (ajaxResult === false) {
                self.showError($("body"), message, self.showtime_error);
                return;
            }
            //if (!ajaxResult.success) {
            //    self.showError($("body"), ajaxResult.errorMessage);
            //} else {
            //    if (message && $.trim(message) != '') {
            //        self.showSuccess($("body"), message, self.showtime_success);
            //    }
            //}
        }
    });

    return $;
});