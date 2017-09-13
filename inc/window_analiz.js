function ShowWindowAnaliz()
{
	var mm = moment().startOf('month');
	webix.ui.datafilter.countRows = webix.extend({
	refresh:function(master, node, value){
    node.firstChild.innerHTML = master.count();
  }
}, webix.ui.datafilter.summColumn);
webix.i18n.setLocale("ru-RU");	

webix.ui({
	view:"window",  id:"wanaliz",left:50, top:50,	move:true, resize: true,
    head:{
		view:"toolbar", cols:[
						{view:"label", label: "Таблица анализа: "},
						
							{ view: "button", label: 'Закрыть', width: 100, align: 'right', click: function () { this.getTopParentView().close(); } },
                
							
						]
	},
	body:{
		 rows:[
					{ cols:[ //2nd row
						{rows:[
							{ view:"radio", customRadio:false, name:"arc", id:"arc", label:"Архив: ", value:1, options:[{ id:1, value:"Часовые" }, { id:2, value:"Суточные" }] },
							{ view:"datepicker", id:"dateFrom", timepicker:true, icons:true, width:300, label:"От", value:moment().startOf('month').format("YYYY.MM.DD HH:mm"), name:"start", stringResult:true, format:"%d  %M %Y %H:%i" },
							{ view: "datepicker", id: "dateTo", timepicker: true, icons:true, value: moment().format("YYYY.MM.DD 23:00"), width: 300, label: "До", name: "end", stringResult: true, format: "%d %M %Y %H:%i" },
							]},
							{ view:"button", value:"Получить", width: 150, click:"GetAnaliz"},
							{ view:"text", id:"border", value:'0', label:"Граница",width: 150,height:40, validate:webix.rules.isNumber, css:"BigFontTextField"},
						]},
					
            ],
	
	}
}).show();
//GetAnaliz();
webix.i18n.setLocale("ru-RU");
}
function GetAnaliz()
{
	var arc = $$("arc").getValue();
	if(arc == 2)
	{
		arc = "days";	
	}
	else{
	arc = "hours"
	}
	var dateFrom = $$("dateFrom").getValue();
	var dateTo = $$("dateTo").getValue();
	var border = $$("border").getValue();
	var url = 'ReportToSTS.asmx/GetCountGVS?arc=' + arc + '&dateFirst="' + dateFrom + '"&dateLast="' + dateTo + '"&border=' + border;
	window.location = url;
}


  
