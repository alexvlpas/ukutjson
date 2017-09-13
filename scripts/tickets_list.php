<?php
header('Content-Type: text/html; charset=utf-8');
#
# 
#  idUkut - номер узла учета в базе данных
#  
#
include("../bd_ticket.php");
$idUkut = $_GET['idUkut'];
if($idUkut != 0){
$json = '[';
$query_channel = "SELECT ost_ticket__cdata.ticket_id, ost_ticket__cdata.aaddress, ost_ticket__cdata.idUkut, ost_ticket_thread.poster, ost_ticket_thread.poster, ost_ticket_thread.body,ost_ticket_thread.created, ost_ticket.number, ost_ticket.closed FROM ost_ticket__cdata
INNER JOIN ost_ticket_thread ON ost_ticket__cdata.ticket_id=ost_ticket_thread.ticket_id
INNER JOIN ost_ticket ON ost_ticket__cdata.ticket_id=ost_ticket.ticket_id
 WHERE  ost_ticket__cdata.idUkut = ";
$query_channel = $query_channel.$idUkut;
    $res_channel = mysql_query($query_channel);
    if (!$res_channel) {
    echo "Could not successfully run query ($query_channel) from DB: " . mysql_error();
    exit;
}

if (mysql_num_rows($res_channel) == 0) {
    echo "No rows found, nothing to print so am exiting";
    exit;
}
    
    while($channel_devices = mysql_fetch_assoc($res_channel))
	{
        $json.= json_encode($channel_devices).',';
    }

    $json = substr($json, 0,-1);
    echo  $json.']';


}
?>