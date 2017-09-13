<?php
	require_once("inc/config.php");
  require("inc/data_connector.php");
	$res=mysql_connect($mysql_server,$mysql_user,$mysql_pass);
	mysql_select_db($mysql_db);

	$grid = new JSONDataConnector($res);
	//$grid->set_options("idMD","!=0");

	$grid->render_table("ukut", "idUkut", "address, geu, Abonent, Dogovor, dQotop, dGgvs, tcw, devName, serialNum, DevType, lon, lat, teplo_source, countTrub, system, idMD, column_gvs, column_pod, column_obr, formula_winter, formula_summer, primechanie, scheme, uchet");
?>x