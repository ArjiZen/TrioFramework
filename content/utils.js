define('utils', ['jquery', 'template', 'bootstrap'], function ($, template) {
    // 新版本的jquery插件工具类，逐步替代jquery.extend.js
    var self = {
        /*显示错误信息*/
        showError: function (elementSelector, message, showtime) {
            $(elementSelector).find('.alert').remove();
            $(elementSelector).append('<div class="alert alert-error"><a class="close" data-dismiss="alert">×</a>' + message + '</div>');
            $(elementSelector).find("div").slideDown();
            if (showtime) {
                if (showtime == 0) {
                    return;
                }
                setTimeout(function () {
                    self.clear(elementSelector);
                }, showtime);
            }
        },
        /*显示操作成功信息*/
        showSuccess: function (elementSelector, message, showtime) {
            $(elementSelector).find('.alert').remove();
            $(elementSelector).append('<div class="alert alert-success"><a class="close" data-dismiss="alert">×</a>' + message + '</div>');
            $(elementSelector).find("div").slideDown();
            if (showtime) {
                if (showtime == 0) {
                    return;
                }
                setTimeout(function () {
                    self.clear(elementSelector);
                }, showtime);
            }
        },
        /*清除成功、错误信息*/
        clear: function (elementSelector) {
            $(elementSelector).find('.alert').slideUp(function () {
                $(elementSelector).find('.alert').remove();
            });
        },
        /*模板绑定*/
        template: function(selector, templateid, data) {
            $(selector).html(template(templateid, data));
        }
    };

    // $("xxx").func()
    $.fn.extend({
        /*绑定Panel展开收缩*/
        panelToggle: function () {
            $.each($(this), function (i, n) {
                $(n).on('click', "label", function () {
                    $(n).find(".content").slideToggle();
                });
            });
        },
        /*绑定下拉列表文本值绑定*/
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
        },
        /*绑定分页控件事件*/
        pagerBinder: function (formSelector) {
            var len = $(this).length;
            if (len == 0) {
                console.error("pagerBinder:未找到分页控件所在的容器");
                return;
            }
            if (len > 1) {
                console.error("pagerBinder:分页控件绑定不支持多个容器");
                return;
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
        },
        /*根据模板绑定table*/
        tableBinder: function (templateid, data) {
            data['empty'] = (data.Option.Total == 0);
            self.template(this, templateid, data);
        },
        /*模板绑定*/
        templateBinder: function(templateid, data) {
            self.template(this, templateid, data);
        },
        /*请求form表单*/
        request: function (callback) {
            var action = $(this).attr('action');
            var parameters = $(this).serialize();
            $.request(action, parameters, callback);
        },
        /*显示执行结果*/
        alert: function (ajaxResult, successMessage) {
            if (!ajaxResult.success) {
                self.showError($(this), ajaxResult.errorMessage);
            } else {
                if (successMessage && $.trim(successMessage) != '') {
                    self.showSuccess($(this), successMessage, 5000);
                }
            }
        }
    });

    $.extend({
        /*Ajax请求获取数据*/
        request: function (url, parameters, callback) {
            $.ajax({
                type: 'POST',
                data: parameters,
                dataType: 'json',
                url: url,
                success: function (result) {
                    var data = $.json(result.data);
                    callback(result, data);
                }
            });
        },
        /*转为json对象*/
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
        }
    });

    return $;
});