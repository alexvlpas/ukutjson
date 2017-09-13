			var map = null,
            polyline,
            data = null,
            clusterer = null,
            FOOTER = null;
            mapTimer = null;
        var sensid = 0,
            past = 0,
            days = 1,
            pmid = 0;
            aaddress = 0;
			geu2 = 0;
        var DEVS = [],
            CAMS = [],
            TRUB = [],
            STYLES = [],
            GEU = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            TYPES = [0, 1, 2, 3, 4, 10];
        MAP_CENTER = [56.894019, 60.596773];
        var VALUES;
        var TEMP_MODE = 1;
        var PERET_MODE = 1;
        var Collection;
        var btnTemp;
        var btnMode;
        STYLES[0] = 'twirl#greyStretchyIcon'; //Нет данных в течение 12 минут
        STYLES[1] = 'twirl#blueStretchyIcon'; //Низко
        STYLES[2] = 'twirl#greenStretchyIcon'; //Норма
        STYLES[3] = 'twirl#redStretchyIcon'; //Высоко
		STYLES[4] = 'twirl#blackStretchyIcon'; //Неисправен ТВЧ
        STYLES[8] = 'twirl#photographerIcon';
        STYLES[9] = 'twirl#photographerIcon';
        STYLES[10] = 'twirl#brownStretchyIcon'; //Электросчетчик
        ymaps.ready(initMap);

        function initMap() {
            map = new ymaps.Map('map', {
                center: MAP_CENTER,
                zoom: 11,
                type: 'yandex#map',
                behaviors: ['default', 'scrollZoom', 'multiTouch']
            });
            map.controls.add('mapTools');
            map.controls.add('zoomControl', {
                left: 7,
                top: 40
            });
            map.controls.add('typeSelector');
            
            
            var btnUpdMap = new ymaps.control.Button({
                data: {
                    image: 'img/reload.png',
                    content: '',
                    title: 'Обновить показания на карте'
                }
            }, {
                selectOnClick: false
            });
            btnUpdMap.events.add('click', GetMarkers);
        
            map.controls.add(btnUpdMap, {
                left: 4,
                top: 230
            });
            var btnHome = new ymaps.control.Button({
            data: {
                image: 'img/table.png',
                content: '',
                title: 'Список УКУТ'
            }
        }, {
            selectOnClick: false
        });
        btnHome.events.add('click', ShowWindowUkuts);
        map.controls.add(btnHome, {
            left: 4,
            top: 260
        });
       
                 //Трубы зулу
         var btnTrubZ = new ymaps.control.Button({
            data: {
                image: 'img/pipe.png',
                content: '',
                title: 'Показать сети'
            }
        }, {
            selectOnClick: false
        });
        btnTrubZ.events.add('click', LoadKml);
        map.controls.add(btnTrubZ, {
            left: 4,
            top: 290
        });
        //Кнопка изменить режим отображения
        var btnChangeMode = new ymaps.control.Button({
            data: {
                image: 'img/search.png',
                content: '',
                title: 'Изменить режим'
            }
        }, {
            selectOnClick: false
        });
        btnChangeMode.events.add('click', SetModeTemp);
        map.controls.add(btnChangeMode, {
            left: 4,
            top: 320
        });
        //Конец режима
        //Кнопка выборки недостоверных значений
        var btnWarningList = new ymaps.control.Button({
            data: {
                image: 'img/warning.png',
                content: '',
                title: 'Неисправности'
            }
        }, {
            selectOnClick: false
        });
        btnWarningList.events.add('click', OpenRepairs);
        map.controls.add(btnWarningList, {
            left: 4,
            top: 350
        });
        //Конец кнопки
       
        //Конец кнопки
       //Кнопка расчета по ГВС
        var btnGVS = new ymaps.control.Button({
            data: {
                image: 'img/report.png',
                content: '',
                title: 'Отчеты'
            }
        }, {
            selectOnClick: false
        });
        btnGVS.events.add('click', OpenReports);
        map.controls.add(btnGVS, {
            left: 4,
            top: 380
        });
            //Конец кнопки
        //Отображение температуры наружного воздуха
        btnTemp = new ymaps.control.Button({
            data: {
               // image: 'img/search.png',
                content: 'Температура наружного воздуха:',
                title: 'Температура наружного воздуха'
            }
        }, {

            selectOnClick: false
        });
        map.controls.add(btnTemp, {
            right: 200,
            top: 5
        });

        var ServiceControl = new ymaps.control.RadioGroup({
            items: [
                new ymaps.control.Button('Старт'),
                new ymaps.control.Button('Стоп'),
            ]
        });
        ServiceControl.get(0).events.add('click', function () {
           // PERET_MODE = 0;
            //GetMarkers();
            AJAX("/ukut.asmx/SendLinesCommand?action=RestartLine", "?action=RestartLine", null);
        });
        ServiceControl.get(1).events.add('click', function () {
           // PERET_MODE = 1;
            //GetMarkers();
            AJAX("/ukut.asmx/SendLinesCommand?action=StopLine", "?action=StopLine", null);
        });
       /* map.controls.add(ServiceControl, {
            right: 1200,
            top: 5
        });
        */
        //Конец отображения
        // Создание радио-группы элементов управления.
        var radioGroup = new ymaps.control.RadioGroup({
        items: [
            new ymaps.control.Button({ data: { content: 't нв', title: 'Выбор температурного графика в соответствии с температурой наружного воздуха' } }),
            // Вторая кнопка будет выбрана, если все остальные кнопки отжаты.
            new ymaps.control.Button({ data: { content: 't под', title: 'Выбор температурного графика в соответствии с температурой подающей трубы' } }, { selectedByDefault: true })
        ]
    });
        radioGroup.get(0).events.add('click', function()
                                     { PERET_MODE = 0;
                                        GetMarkers();
                                     });
        radioGroup.get(1).events.add('click', function()
                                     { PERET_MODE = 1;
                                        GetMarkers();
                                     });
        map.controls.add(radioGroup, { left: 10, bottom: 80 });
        // Изначально будет нажата первая кнопка.
        radioGroup.get(1).select();
        //Отображение режима работы
        btnMode = new ymaps.control.Button({
            data: {
               // image: 'img/search.png',
                content: '<b>Режим:</b> температура ГВС',
                title: 'Режим отображения'
            }
        }, {
            selectOnClick: false
        });
        btnMode.events.add('click', SetModeTemp);
        map.controls.add(btnMode, {
            right: 600,
            top: 5
        });
        //Конец отображения
        map.setCenter(MAP_CENTER, 16, {
                checkZoomRange: true,
                callback: GetMarkers
            });
            var iconHTML = '<span title="Показать узлы в этой области">';
            var iconLayout = ymaps.templateLayoutFactory.createClass('', {
                build: function () {
                    iconLayout.superclass.build.call(this);
                    var content = "",
                        pubval = "",
                        minval = 70;
                    var objs = this.getData().properties.get('geoObjects');
                    for (var i = 0; i < objs.length; i++) {
                        pubval = objs[i].properties.get('pubval');
                        if ((pubval == "") || isNaN(pubval) || (pubval == 0)) continue;
                        if (pubval < minval) minval = pubval;
                       
                    }
                    if (minval < 40) minval += '°';
                    else if (minval <= 100) minval += '%';
                    if (minval == 999) minval = '&nbsp;УКУТ&nbsp;';
                     minval = pubval;
                    this.getParentElement().innerHTML = iconHTML + pubval + '</span>';
                }
            });
            var itemLayout = ymaps.templateLayoutFactory.createClass(['<div class=entry>',
                                                                      '<div class="entry">$[properties.balloonContentHeader]</div>',
                                                                      '<div">$[properties.balloonContentBody]</div>',
                                                                       '<div>$[properties.balloonContentFooter]</div>',
                                                                      '</div>'
                                                                      ].join(''));
            clusterer = new PieChartClusterer({
            //clusterer = new ymaps.Clusterer({
                gridSize: 64,
                margin: 32,
                minClusterSize: 2,
                //clusterNumbers: [999],
                clusterBalloonWidth: 400,
                clusterBalloonHeight: 800,
                synchAdd: true,
                clusterIconContentLayout: iconLayout,
                clusterBalloonContentItemLayout: itemLayout,
                clusterBalloonContentBodyLayout: "cluster#balloonAccordionContent"    
            });        
        Collection = new ymaps.GeoObjectCollection();
        map.geoObjects.add(Collection);
        //Слои KML
        setInterval(ShowMarkers, 50000);

        }
        function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                vars[key] = value;
            });
            return vars;
        }

        function onGeoXmlLoad (res) {
        map.geoObjects.add(res.geoObjects);
             if (res.mapState)
                 {
                     res.mapState.applyToMap(map);
                 }
        }
        function LoadKml()
        {
            ymaps.geoXml.load('http://poverka-ekb.ru/166.kml').then(onGeoXmlLoad);
           ymaps.geoXml.load('http://poverka-ekb.ru/175.kml').then(onGeoXmlLoad); 
        }
        function ShowObj(markers, shown) {
            markers.forEach(function (PM) {
                PM.options.set('visible', shown);
            });
        }
        function SetModeTemp()
        {
            if (TEMP_MODE === 1) {
               TEMP_MODE = 10;
               GetMarkers();
               btnMode.data.set('content','<b>Режим:</b> температура обратки');
            }
            else
            {
               TEMP_MODE = 1;
               GetMarkers();
               btnMode.data.set('content','<b>Режим:</b> температура ГВС');
            }
            
            
        }
        function SetModeViewPeretops()
        {
            if (PERET_MODE === 0) {
               PERET_MODE = 1;
               GetMarkers();
               btnMode.data.set('content','<b>Режим:</b> температура по подаче');
            }
            else{
               PERET_MODE = 0;
               GetMarkers();
               btnMode.data.set('content','<b>Режим:</b> температура как обычно');
            }
        }
        function SetShown(type, clear, shown) {
            if (clear) {
                TYPES.forEach(function (idx) {
                    GetObj('chk' + idx).checked = false;
                });
                if (type >= 0) GetObj('chk' + type).checked = true;
                TYPES = [];
                GetObj('chkcam').checked = (type < 0);
                ShowObj(CAMS, (type < 0));
                ShowObj(DEVS, false);
            }
            if (type >= 0 && (clear || shown)) TYPES.push(type);
			if (type < 0 && !clear) ShowObj(CAMS, shown);
            if (type >= 0 && !clear && !shown) {
                ShowObj(DEVS, false);
                for (var i = 0; i < TYPES.length; i++) {
                    if (TYPES[i] == type) TYPES.splice(i, 1);
                }
				
            }
            GetMarkers();
        }
        function ReloadMarkers(checked) {
            if (checked) {
                ShowObj(CAMS, false);
                ShowObj(DEVS, false);
            }
            GetMarkers();
        }

        function GetPM(id) {
            if (id > 0 && DEVS[id]) return DEVS[id];
            if (id < 0 && CAMS[-id]) return CAMS[-id];
            return false;
        }

        function GetMarkers() {
            if (mapTimer) clearTimeout(mapTimer);
            var params = '?ukuts=all';
            var url = "/ukut.asmx/UkutsMaps";
            url += "?res_mode=maps&type=" + TYPES.join(",");
            url += "&temp_mode=" + TEMP_MODE;
            url += "&peret_mode=" + PERET_MODE;
            url += "&geu=" + GEU.join(",");
            AJAX(url, params, ShowMarkers);
        }
        function GetTnv() {
            var params = '';
            var url = "/ukut.asmx/GetTnvWeb";
            AJAX2(url, params, InsertTnv);
        }
        function InsertTnv() {
            if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {
                var tnvArray = JSON.parse(xmlHttp.responseText);
                btnTemp.data.set('content', tnvArray[0].channel_name + ': <b>' + tnvArray[0].channel_value + '</b>');
            }
        }
     
        function ShowMarkers() {
            if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {
                HideStatusDiv();
                if (xmlHttp.responseText == "") {
                    mapTimer = setTimeout(GetMarkers, 5 * 60 * 100);
                    return;
                }
                markers = eval(xmlHttp.responseText);
                while (markers.length > 0) {
                    var marker = markers.shift();
                    var id = parseInt(marker[10]),
                        type = parseInt(marker[6]);
                    var lat = marker[1],
                        lng = marker[2],
                        updated = marker[3],
                        address = marker[8],
                        geu = marker[9];
                        idUkut = marker[10],
                        pointNum = marker[11],
                        idMD = marker[12];
                        system = marker[13];
                        channels = marker[14];
						zabbix_status = marker[15];
						zabbix_time = marker[16];
						otkl = marker[17];
						teplo_source = marker[18];
						regulat =   marker[19];
						idDevType = marker[20];
                        status = marker[21];
                    AddMarker(id, type, lat, lng, marker[3], marker[4], marker[5], marker[7], address, geu, idUkut, pointNum, idMD, system,channels, teplo_source, updated, zabbix_status, zabbix_time, otkl, regulat,idDevType, status);
                }
               
                if (!clusterer.getMap()) map.geoObjects.add(clusterer);
                mapTimer = setTimeout(GetMarkers, 5 * 60 * 100);
                
                OpenBaloonGet();
                
            } else AJAXStatus("Обновление показаний...");
          
        }
        function OpenBaloonGet() {
            var idUkut = getUrlVars()["idUkut"];
            if (idUkut) {
                ZoomIn(idUkut);
                var query = window.location.search.substring(1)

                if (query.length) {
                    if (window.history != undefined && window.history.pushState != undefined) {
                        window.history.pushState({}, document.title, window.location.pathname);
                    }
                }
                var PM = GetPM(idUkut);
               
            }
        }
        function AddMarker(id, type, lat, lng, time, pubval, txt, temp_graph, address, geu, idUkut, pointNum, idMD, system, channels, teplo_source, updated, zabbix_status, zabbix_time, otkl, regulat,idDevType, status) {
            var up_date = updated;
            updated = moment(updated, "YYYY-MM-DD hh:mm:ss").fromNow();
           
            if (type === 0)
            {
                updated = up_date;
            }

		   zabbix_time = moment(zabbix_time,"YYYY-MM-DD hh:mm:ss").fromNow();
		 //  var img_stat = "<span class='sImg' title='Нет связи с конвертором. Последний сеанс связи был: "+zabbix_time+"' style='background-position:-64px -16px'><img src='/img/pix.gif' width='16' /></span>";
		   var img_regulat = ' <a href="#" onclick=ShowDialogRegulator()><img src="img/umbrella-corp-icon.png" alt="Регулятор"></a>';
		 //  if (zabbix_status === "OK")
		  // {
			//img_stat = "<span class='sImg' title='Связь с конвертором есть!' style='background-position:-192px -16px'><img src='/img/pix.gif' width='16' /></span>";
		  // }
		  if (regulat != 1)
		   {
			img_regulat = ' ';
		   }
          var header = '<b> ' + address + '</b>' + img_regulat + '<br>' + 'ЖЭУ №: ' + geu + '<br>Количество труб: ' + pointNum + '<br>Система: ' + system + '<br>Прибор: ' + pubval + '<br>Источник: ' + teplo_source + '<br>График t°C: ' + temp_graph + '<br>Обновлено: ' + updated + '<br>Статус: ' + status;
   
		    if (otkl != "Нет отключений")
		   {
			header = header+'<br>'+otkl;
		   }
			if (type === 10) {

                header = '<b><br>'+address+'</b><br>Прибор: '+pubval;
            }
            var GP = [lat, lng],
                PM = GetPM(id);
                var gvs = 'Температура ГВС: ' + txt;
            if (PM == false) {
                PM = new ymaps.Placemark(GP,{
                    id: id,
                    idMD: idMD,
					idDevType: idDevType,					
                    time: time,
                    address: address,
                    system: system,
                    ns_flag: 0,
                    geu: geu,
                    loaded: 0,
                    type: type,
                    teplo_source: teplo_source,
                    temp_graph: temp_graph,
                    hintContent: address
                }    
                
                , {
                    preset: STYLES[type],
                    hideIconOnBalloonOpen: true
                });
                
                var channels_body = PMBody(channels);
                var custom_footer = PMFooter(id, type, pubval, temp_graph, address, geu, time, idDevType);
                PM.properties.set('balloonContentFooter', custom_footer);
                PM.properties.set('balloonContentBody', channels_body);
                PM.properties.set('balloonContentHeader', header);
		
				PM.properties.set('iconContent',txt);
                PM.events.add('balloonopen', onBalloonOpen);
                PM.events.add('balloonclose', function (e) {
                    pmid = 0;
                    aaddress = 0;
                });
                if (id < 0) {
                    CAMS[-id] = PM;
                    map.geoObjects.add(PM);
                }
                if (id > 0) {
                    PM.properties.set('pubval', txt);
                    PM.properties.set('iconContent', txt);
                    DEVS[id] = PM;
                    clusterer.add(PM);
                    
                }
                return;
            }
            
            if (id != pmid) PM.properties.set('loaded', 0);
            PM.properties.set('time', time);
            if (id > 0) {
                PM.options.set('preset', STYLES[type]);
                PM.properties.set('iconContent',txt);
                PM.properties.set('pubval', txt);
                PM.properties.set('ns_flag', '1');
                var channels_body = PMBody(channels);
                var custom_footer = PMFooter(id, type, pubval, temp_graph, address, geu, time, idMD);
                PM.properties.set('balloonContentHeader', header);
                PM.properties.set('balloonContentBody', channels_body);
            }
            var GP0 = PM.geometry.getCoordinates();
            if ((GP[0] != GP0[0]) || (GP[1] != GP0[1])) {
                if (id > 0) clusterer.remove(PM);
                PM.geometry.setCoordinates(GP);
                if (id > 0) clusterer.add(PM);
            }
            if (PM.balloon.isOpen()) return;
            if (PM.options.get('visible') == false) PM.options.set('visible', true);
            return;
        
        }
        
        onBalloonOpen = function (e) {
            var PM = e.get('target');
            pmid = PM.properties.get('id');
            aaddress = PM.properties.get('address');
			geu2 = "ЖЭУ "+PM.properties.get('geu');
        };
        function PMFooter(id, type, pubval, temp_graph, address, geu, time, idDevType) {
             var footer = "<a class='hover' onclick='ZoomIn(" + id + ")' href='#'>Приблизить</a>";
             footer += "<a class='hover' onclick='ShowWindowTickets(" + id + ")' href='#'>, Заявки</a>";
             
             if (idDevType === '1') {
               footer += "<a class='hover' onclick='GetPMonArchive(" + id + ", 1)' href='#'>, Архивы</a>&nbsp;";
               footer += "<a class='hover' onclick='ShowWindowArchivesIzm(" + id + ", 1)' href='#'>, Изм. БД</a>&nbsp;";
           }
             if (idDevType === '3' || idDevType === '2' || idDevType === '4' || idDevType === '5' || idDevType === '6') {
               footer += "<a class='hover' onclick='GetPMonArchive(" + id + ", 1)' href='#'>, Архивы</a>&nbsp;";
           }
            return footer;
        }
        function PMBody(channels) {
            var ch = channels.split(';');
             var body = "<b>Текущие параметры:</b><br>";
             var ch_e;
             var i =0;
            while(ch.length-1 > 0){
                i++;
                var ch_r = ch.shift();
                ch_e = ch_r.split(':');
                if (ch_e[1] != 'undefined') {
                body += '<b>'+i+'. '+ch_e[1]+'</b>:<font color='+ch_e[4]+'>'+ch_e[3]+'<br></font>';
                //    if (ch_e[2] == 470) {
                  //   btnTemp.data.set('content',ch_e[1]+': <b>'+ch_e[3]+'</b>');
                //    }
                }
            }
            return body;
        }
        function ZoomIn(id) {
            var PM = GetPM(id);
            if (PM){
            map.setBounds(PM.geometry.getBounds(), {
                checkZoomRange: true
            });
            //PM.balloon.open();
           } 
        }

        function ZoomOut(id) {
            var PM = GetPM(id);
            if (PM) map.setCenter(PM.geometry.getCoordinates(), 11, {
                checkZoomRange: true
            });
        }
        function OpenReports()
        {
            window.location = '/reports/';
        }
        function OpenRepairs() {
            window.location = '/repair/';
        }