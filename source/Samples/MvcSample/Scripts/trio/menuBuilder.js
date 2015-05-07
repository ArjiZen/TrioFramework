define(['jquery.extend', 'jsrender', 'jquery.cookie', 'ztree', 'ztree.excheck'], function ($) {

    var curExpandNode = null;
    function beforeExpand(treeId, treeNode) {
        var pNode = curExpandNode ? curExpandNode.getParentNode() : null;
        var treeNodeP = treeNode.parentTId ? treeNode.getParentNode() : null;
        var zTree = $.fn.zTree.getZTreeObj("sidemenu");
        for (var i = 0, l = !treeNodeP ? 0 : treeNodeP.children.length; i < l; i++) {
            if (treeNode !== treeNodeP.children[i]) {
                zTree.expandNode(treeNodeP.children[i], false);
            }
        }
        while (pNode) {
            if (pNode === treeNode) {
                break;
            }
            pNode = pNode.getParentNode();
        }
        if (!pNode) {
            singlePath(treeNode);
        }

    }
    function singlePath(newNode) {
        if (newNode === curExpandNode) return;

        var zTree = $.fn.zTree.getZTreeObj("sidemenu"),
                rootNodes, tmpRoot, tmpTId, i, j, n;

        if (!curExpandNode) {
            tmpRoot = newNode;
            while (tmpRoot) {
                tmpTId = tmpRoot.tId;
                tmpRoot = tmpRoot.getParentNode();
            }
            rootNodes = zTree.getNodes();
            for (i = 0, j = rootNodes.length; i < j; i++) {
                n = rootNodes[i];
                if (n.tId != tmpTId) {
                    zTree.expandNode(n, false);
                }
            }
        } else if (curExpandNode && curExpandNode.open) {
            if (newNode.parentTId === curExpandNode.parentTId) {
                zTree.expandNode(curExpandNode, false);
            } else {
                var newParents = [];
                while (newNode) {
                    newNode = newNode.getParentNode();
                    if (newNode === curExpandNode) {
                        newParents = null;
                        break;
                    } else if (newNode) {
                        newParents.push(newNode);
                    }
                }
                if (newParents != null) {
                    var oldNode = curExpandNode;
                    var oldParents = [];
                    while (oldNode) {
                        oldNode = oldNode.getParentNode();
                        if (oldNode) {
                            oldParents.push(oldNode);
                        }
                    }
                    if (newParents.length > 0) {
                        zTree.expandNode(oldParents[Math.abs(oldParents.length - newParents.length) - 1], false);
                    } else {
                        zTree.expandNode(oldParents[oldParents.length - 1], false);
                    }
                }
            }
        }
        curExpandNode = newNode;
    }

    function onExpand(event, treeId, treeNode) {
        curExpandNode = treeNode;
    }

    function onClick(e, treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj("sidemenu");
        zTree.expandNode(treeNode, null, null, null, true);
    }

    var builder = {
        menu: {
            _cacheMenu: null,
            _url: '',
            _thisMenu: null,
            init: function (menus, url) {
                builder.menu._cacheMenu = menus;
                builder.menu._url = url;
                builder.menu._thisMenu = builder.menu._findMenuByUrl(menus, url);
                builder.menu.render();
            },
            render: function () {/*渲染菜单*/
                //自动发现
                if ($("#topmenu_template")[0]) {
                    builder.menu._renderTop();
                } else {
                    builder.menu._renderLeft();
                }
            },
            _findMenu: function (menus, id) {
                for (var i = 0; i < menus.length; i++) {
                    if (menus[i].id === id) {
                        return menus[i];
                    }

                    if (menus[i].children) {
                        var _ = builder.menu._findMenu(menus[i].children, id);
                        if (_) return _;
                    }
                }
                return null;
            },
            _findMenuByUrl: function (menus, url) {
                for (var i = 0; i < menus.length; i++) {
                    var offset = (url + '!').indexOf(menus[i].url + '!');
                    if (offset > 0 && offset < url.length) {
                        return menus[i];
                    }
                    if (menus[i].children) {
                        var _ = builder.menu._findMenuByUrl(menus[i].children, url);
                        if (_) return _;
                    }
                }
                return null;
            },
            _refind: function (children, menuid) {
                if (children) {
                    for (var i = 0; i < children.length; i++) {
                        if (children[i].id === menuid) {
                            return children[i];
                        } else {
                            var menu = builder.menu._refind(children[i].children, menuid);
                            if (menu) {
                                return children[i];
                            }
                        }
                    }
                } else {
                    return null;
                }
            },
            _findTopMenu: function (menus, menuid) {
                return builder.menu._refind(menus, menuid);
            },
            _findLeftMenu: function (menus, menuid) {
                return builder.menu._refind(menus, menuid);
            },
            _renderTop: function () {
                var menus = builder.menu._cacheMenu;
                var container = $("#topmenu_template").attr("container");
                if (builder.menu._cacheMenu === '') {
                    $(container).html("<ul class='nav'></ul>");
                    builder.layout.init();
                    return;
                }
                $(container).html(
                    builder.menu._cacheMenu === '' ? "<ul class='nav'></ul>" : $("#topmenu_template").render(builder.menu._cacheMenu)
                ).find("a").click(function () {
                    $(container).find("li").removeClass("active");
                    $(this).parents("li").addClass("active");
                    var menuid = $(this).attr('menuid');

                    // renderLeftMenu
                    var menu = builder.menu._findMenu(menus, menuid);

                    if (menu && menu.children && menu.children.length > 0) {
                        $("#sidebar").show();
                        $(".wrapper").removeClass("nosidebar");
                        builder.menu._renderLeft(menu.children);
                        // 切换TopMenu时默认打开第一个菜单，如果当前页面存在
                        var leftContainer = $("#leftmenu_template").attr("container");
                        var firstMenu = $(leftContainer).find('a:first');
                        var thisMenu = builder.menu._thisMenu;
                        if (thisMenu) {
                            if ($(leftContainer).find("a[menuid='" + thisMenu.id + "']").length === 0) {
                                if (firstMenu.parent().hasClass('current') === false) {
                                    firstMenu.click();
                                }
                            } else {
                                $(container).find("a[menuid='" + thisMenu.id + "']").parents("li").addClass("current");
                            }
                        } else {
                            if (firstMenu.parent().hasClass('current') === false) {
                                firstMenu.click();
                            }
                        }
                    } else {
                        $("#sidebar").hide();
                        $(".wrapper").addClass("nosidebar");
                        builder.layout.init();
                    }
                    return false;
                });

                var thisMenu = builder.menu._thisMenu;
                if (thisMenu) {
                    var topMenu = builder.menu._findTopMenu(menus, thisMenu.id);
                    $(container).find("a[menuid='" + topMenu.id + "']").click();
                } else {
                    $(container).find("a:first").click();
                }

            },
            _renderLeft: function (menus) {
                menus = menus || builder.menu._cacheMenu;
                $.fn.zTree.init($("#sidemenu"), {
                    view: {
                        selectedMulti: false,
                        showIcon: false,
                        showLine: false,
                        dblClickExpand: true
                    },
                    check: {
                        enable: false
                    },
                    callback: {
                        beforeExpand: beforeExpand,
                        onExpand: onExpand,
                        onClick: onClick
                    }
                }, menus);

                setTimeout(function () {
                    var treeObj = $.fn.zTree.getZTreeObj("sidemenu");
                    var nodes = null;
                    var thisMenu = builder.menu._thisMenu;
                    if (thisMenu) {
                        var currentNode = treeObj.getNodesByParam("id", thisMenu.id, null);
                        if (currentNode.length > 0) {
                            var pNode = currentNode[0];
                            do {
                                pNode = pNode.getParentNode();
                                if (pNode == null) {
                                    break;
                                }
                                treeObj.expandNode(pNode, true, false, true);
                            } while (pNode);
                            $("#sidemenu a[href='" + thisMenu.url + "']").addClass("curSelectedNode");
                        } else {
                            nodes = treeObj.getNodes();
                            if (nodes.length > 0) {
                                treeObj.expandNode(nodes[0], true, false, true);
                            }
                        }
                    } else {
                        nodes = treeObj.getNodes();
                        if (nodes.length > 0) {
                            treeObj.expandNode(nodes[0], true, false, true);
                        }
                    }
                }, 100);

                builder.layout.init();
            },
            _navigate: function (id, url) {
                if (url !== '#') {
                    location.href = url;
                }
            }
        },
        layout: {
            init: function () {
                // 动态设置右边内容高度
                var contentHei = $(window).height() - $(".header").outerHeight(true) - $(".footer").outerHeight(true) - ($(".pageContent").outerHeight(true) - $(".pageContent").height());

                var contentTop = $(".pageContent").css("top");
                if ($(".pageContent").css("top") === "auto") contentTop = "0px";

                $("#sidebar").height(contentHei); //.css("top", top + "px")
                $(".pageContent").height(contentHei);

                //窗口改变后动态计算
                $(window).unbind("resize").bind("resize", function () {
                    var conHei = $(window).height() - $(".header").height() - $(".footer").height() - 110;
                    $("#sidebar").height(conHei);
                    $(".pageContent").height(conHei);
                });
            }
        }
    }

    return {
        initMenu: function (menus, thisUrl) {
            builder.menu.init(menus, thisUrl);
        },
        initLayout: function () {
            builder.layout.init();
        }
    }
})