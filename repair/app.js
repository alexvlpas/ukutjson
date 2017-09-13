function ShowWindowUkuts() {
    webix.ui.datafilter.countRows = webix.extend({
        refresh: function (master, node, value) {
            node.firstChild.innerHTML = master.count();
        }
    }, webix.ui.datafilter.summColumn);

    webix.ui({
        view: "window", id: "utickets", move: true, fullscreen: true,
        head: {
            view: "toolbar", cols: [
                            { view: "label", label: "Неисправности УКУТ" },

                            {
                                view: "button", label: 'Excel', width: 100, align: 'right', click: function () {
                                    webix.toExcel($$("grid_ukuts"), {
                                        filterHTML: true
                                    });
                                }
                            },
                                { view: "button", label: 'На карту', width: 100, align: 'right', click: function () { window.open("http://") } },
            ]
        },
        body: {
            rows: [
                    //{ view: "segmented", id: "order_filter", width: 400, labelWidth: 200, align:"center", options: order_filter, label: "Неисправные объекты: ", batch: "grid_ukuts" },
                   {
                       view: "datatable",
                       id: "grid_ukuts",  select: "row",

                       columns: [
                           { id: "idUkut", header: "#", width: 50,  footer: { text: "Всего:", colspan: 1 }},
                           { id: "address", header: ["Адрес", { content: "textFilter" }], width: 200, sort: "string", footer: { content: "countRows" } },
                           { id: "geu", header: ["ЖЭУ", { content: "selectFilter" }], width: 50, sort: "int", css: { 'text-align': 'center' } },
                           { id: "idMD", header: "idMD", hidden: true },
                           { id: "devName", header: ["Прибор", { content: "textFilter" }], width: 160, sort: "string" },
                           {
                               id: "date", header: "Обновлено",
                               template: function (obj) {
                                   var date = moment(obj.date).fromNow();
                                   return date;
                               },
                               adjust: true, sort: "date", format: webix.i18n.dateFormatStr
                           },
                           { id: "countTrub", header: ["Труб", { content: "selectFilter" }], width: 50, sort: "int", css: { 'text-align': 'center' } },
                           { id: "repair_ukut", header: ["Неисправность", { content: "selectFilter" }],  width: 120 },
                           {
                               id: "channels_struct", header: "Каналы", adjust:true, template: function (obj, common) {
                                   var ret=" ";
                                   var keys = Object.keys(obj.channels_struct)
                                   for (var i = keys.length; i--;) {
                                       ret += obj.channels_struct[keys[i]].channel_name + ": <font color=" + obj.channels_struct[keys[i]].repair_color + ">" + obj.channels_struct[keys[i]].channel_value + "</font><br>\n"
                                   }
                                   return ret;
                               }
                           },
                     

                       ],
                       sort: {
                           by: "#address#",
                           dir: "asc",
                           as: "string"
                       },
                       on: {
                           "onItemDblClick": webix.once(function (id, e, node) {
                               var item = this.getItem(id);
                           }),
                           "onLoadError": webix.once(function () {
                               this.hide();
                           }),
                           "onBeforeLoad": function () {
                               this.showOverlay("Загрузка...");
                           },
                           "onAfterLoad": function () {
                               this.hideOverlay();
                               this.adjustRowHeight("channels_struct", true);
                           },

                       },
                       scheme: {
                           $change: function (item) {
                               if (item.idUkut === 'Завышение') {
                                   item.$css = "highlight";
                               }
                               if (item.idUkut === 'Нет связи') {
                                   item.$css = "errlight";
                               }
                               if (item.idUkut === 'Занижение') {
                                   item.$css = "lowlight";
                               }
                           }
                       },
                     //  autoheight: true,
                       //fixedRowHeight: false,
                       resizeRow:true,
                      // rowLineHeight: 15, rowHeight: 135,
                       //autowidth: true,
                       footer: true,
                       //checkboxRefresh: true
                       //autoheight:true
                   }
            ],

        }
    }).show();
    webix.extend($$("grid_ukuts"), webix.ProgressBar);
    $$("grid_ukuts").load("/ukut.asmx/UkutsMaps?type=0,1,2,3,4,10&temp_mode=1&peret_mode=1&geu=1,2,3,4,5,6,7,8,9,10&res_mode=table");
    $$("grid_ukuts").sort("#address#", "asc", "string");
    //$$("order_filter").attachEvent("onChange", function () {
    //    var val = this.getValue();
    //    $$("grid_ukuts").filter(function (obj) {
     //       return obj.repair_ukut == val;
     //   });
   // });
}
function custom_checkbox(obj, common, value) {
    if (value)
        return "<div class='webix_table_checkbox checked'> ДА </div>";
    else
        return "<div class='webix_table_checkbox notchecked'> НЕТ </div>";
};
var order_filter = [
    { id: "Да", value: "Да" },
    { id: "Нет", value: "Нет" }
];
