webix.ready(function () {

    webix.protoUI({
        name: "activeList"
    }, webix.ui.list, webix.ActiveContent);

    webix.i18n.setLocale("ru-RU");
    webix.ui({
        rows: [
            {
                view: "toolbar", id: "toolbar", elements: [
                {
                    view: "icon", icon: "bars",
                    click: function () {
                        if ($$("menu").config.hidden) {
                            $$("menu").show();
                        }
                        else
                            $$("menu").hide();
                    }
                },
                {
                    view: "label",
                    label: "Управление"
                }

                ]
            },
                             {
                                 view: "checkbox", labelWidth: 180, label: "Все", value: 1,
                                 on: {
                                     "onChange": function (newv, oldv) {
                                         if (newv == 0) {
                                             $$("objectsListSTS").data.each(function (obj) {
                                                 // titles.push(obj.title);
                                                 obj.markCheckbox = 0;
                                             });
                                             $$("objectsListSTS").refresh();
                                         }
                                         if (newv == 1) {
                                             $$("objectsListSTS").data.each(function (obj) {
                                                 // titles.push(obj.title);
                                                 obj.markCheckbox = 1;
                                             });
                                             $$("objectsListSTS").refresh();
                                         }
                                     },

                                 },
                             },
                                         {
                                             height: 35,
                                             view: "toolbar",
                                             hide: true,
                                             id: "tools_filter_STS",
                                             elements: [
                                              {
                                                  view: "text", id: "filter_list_STS", label: "Фильтр по адресу", css: "fltr", labelWidth: 170,
                                              }
                                             ]
                                         },
        dt,
          { view: "button", id: "StartArchives", value: "Опросить", click: "StartCustomArchives" },
        ]
    });

    webix.ui({
        view: "sidemenu",
        id: "menu",
        width: 200,
        position: "left",
        state: function (state) {
            var toolbarHeight = $$("toolbar").$height;
            state.top = toolbarHeight;
            state.height -= toolbarHeight;
        },
        css: "my_menu",
        body:{
            view: "list",
            borderless: true,
            scroll: true,
            width: 300,
            height: 400,
            template: "<span class='webix_icon fa-#icon#'></span> #value#",
            data: [
               // { id: 1, value: "Customers", icon: "user" },
                //{ id: 2, value: "Products", icon: "cube" },
              //  { id: 3, value: "Reports", icon: "line-chart" },
               // { id: 4, value: "Archives", icon: "database" },
                { id: 5, value: "Опрос", icon: "cog" }
            ],
            select: true,
            type: {
                height: 40
            },
            on: {
                "onItemClick": function (id, e, node)
                {
                    var item = this.getItem(id);
                    GetMenu(item);
                },
            }
        }
    });
    $$("objectsListSTS").attachEvent("onKeyPress", function (code, e) {
        if (code === 32) {
            var id = $$("objectsListSTS").getSelectedId();
            var item = $$("objectsListSTS").getSelectedItem();
            item.markCheckbox = item.markCheckbox ? 0 : 1;
            $$("objectsListSTS").updateItem(id, item);
        }
    });
    $$("filter_list_STS").attachEvent("onTimedKeyPress", function () {
        var value = this.getValue().toLowerCase();
        $$("objectsListSTS").filter(function (obj) {
            return obj.address.toLowerCase().indexOf(value) == 0;
        })
    });
});
function GetMenu(menuValue)
{
    if (menuValue.value === "Опрос") {
        //$$("wide").load("/reports/");
    }
}
function AJAX(url, params, OnChange) {
    xmlHttp = GetXmlHttpObject(); if (xmlHttp == null) return;
   // bindElement(xmlHttp, 'loadend', GetTnv);
    if (OnChange != null) xmlHttp.onreadystatechange = OnChange;
    xmlHttp.open('GET', url, true);
    xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xmlHttp.send(params);
}
function GetXmlHttpObject() {
    var xmlHttp;
    try { xmlHttp = new ActiveXObject('Msxml2.XMLHTTP'); }
    catch (e) {
        try { xmlHttp = new ActiveXObject('Microsoft.XMLHTTP'); }
        catch (E) { xmlHttp = false; }
    }
    if (!xmlHttp && typeof XMLHttpRequest != 'undefined') { xmlHttp = new XMLHttpRequest(); }
    return xmlHttp;
}
function bindElement(el, event, func) {
    if (el.addEventListener)
        el.addEventListener(event, func, false);
    else if (window.attachEvent)
        el.attachEvent("on" + event, func);
    else
        console.log('error binding event');
}
function StartCustomArchives()
{
    var objects = [];
    $$("objectsListSTS").data.each(function (obj) {
        if ("markCheckbox" in obj) {
            if (obj.markCheckbox == 1) {
                objects.push(obj.idMD);
            }
        }
    });
    if (objects.length > 0) {

        var url = '/ukut.asmx/StartCustomArchives?KPNums=' + objects.join(",");
        // window.location = url;
       // AJAX("/ukut.asmx/SendLinesCommand?action=RestartLine", "?action=RestartLine", Start(url));
        AJAX(url, '', '');
        $$("StartArchives").disable();
    }
    else {
        webix.message({
            type: "error",
            text: "Не выбран ни один объект",
            expire: 1000
            //expire:-1   for canceling expire period
        });
    }
}
function Start(url)
{
    AJAX(url, '', '');
}
var dt = {

    view: "activeList",
    id: "objectsListSTS",
    width: 450,
    select: true,
    //hide: true,
    url: "/ukut.asmx/GetObjectListForReports",
    type: {
        markCheckbox: function (obj) {
            return "<span class='check webix_icon fa-" + (obj.markCheckbox ? "check-" : "") + "square-o'></span>";
        }
    },
    onClick: {
        "check": function (e, id) {
            var item = this.getItem(id);
            item.markCheckbox = item.markCheckbox ? 0 : 1;
            this.updateItem(id, item);
        }

    },
    template: "#idMD#. #address#{common.markCheckbox()}",

}

