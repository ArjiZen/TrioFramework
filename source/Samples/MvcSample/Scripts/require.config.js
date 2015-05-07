/*配置当前站点的虚拟目录名称*/
window.config = {
    baseUrl: ""
};

require.config({
    baseUrl: window.config.baseUrl + '/Scripts',
    paths: {
        'jquery': 'libs/jquery-1.8.2.min',
        'bootstrap': 'libs/bootstrap.min',
        'validator': 'libs/jquery.validate.min',
        'jsrender': 'libs/jsrender.min',
        'ztree': 'libs/jquery.ztree.core.min',
        'ztree.excheck': 'libs/jquery.ztree.excheck.min',
        'template': 'libs/artTemplate',
        'jquery.form': 'libs/jquery.form.min',
        'jquery.layout': 'libs/jquery.layout.min',
        /*Trio Framework Libs*/
    },
    shim: {
        'bootstrap': {
            deps: ['jquery'] // 依赖关系
        },
        'validator': {
            deps: ['jquery']
        },
        'jsrender': {
            deps: ['jquery']
        },
        'jquery.cookie': {
            deps: ['jquery']
        },
        'jquery.layout': {
            deps: ['jquery', 'jquery.extend']
        },
        'ztree': {
            deps: ['jquery']
        },
        'ztree.excheck': {
            deps: ['ztree']
        },
        'jquery.ztree.excheck': {
            deps: ['jquery.ztree.core']
        },
        'template': {
            deps: ['jquery']
        },
        'jquery.form': {
            deps: ['jquery']
        },
        'superTables': {
            deps: ['jquery']
        }
    }
});