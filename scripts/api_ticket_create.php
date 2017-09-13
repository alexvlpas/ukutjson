<?php
#
# Configuration: Enter the url and key. That is it.
#  url => URL to api/task/cron e.g #  http://yourdomain.com/support/api/tickets.json
#  key => API's Key (see admin panel on how to generate a key)
#
$note = $_GET['note'];
$source = $_GET['source'];
$aaddress = $_GET['aaddress'];
$pmid = $_GET['pmid'];
$email = $_GET['email'];
$config = array(
        'url'=>'http://10.235.63.252/ticket/api/tickets.json',
        'key'=>'0445FC034F23145495144F5A70869B7C'
        );

# Fill in the data for the new ticket, this will likely come from $_POST.

$data = array(
    'idUkut'	=>	$pmid,
    'name'      =>      $source,
    'email'     =>      $email,
    'subject'   =>      $aaddress,
    'message'   =>      $note,
    'aaddress'  =>	$aaddress,
    'ip'        =>      $_SERVER['REMOTE_ADDR'],
    'attachments' => array(),
);

/* 
 * Add in attachments here if necessary

$data['attachments'][] =
array('filename.pdf' =>
        'data:image/png;base64,' .
            base64_encode(file_get_contents('/path/to/filename.pdf')));
 */

#pre-checks
function_exists('curl_version') or die('CURL support required');
function_exists('json_encode') or die('JSON support required');

#set timeout
set_time_limit(30);

#curl post
$ch = curl_init();
curl_setopt($ch, CURLOPT_URL, $config['url']);
curl_setopt($ch, CURLOPT_POST, 1);
curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($data));
curl_setopt($ch, CURLOPT_USERAGENT, 'osTicket API Client v1.9');
curl_setopt($ch, CURLOPT_HEADER, FALSE);
curl_setopt($ch, CURLOPT_HTTPHEADER, array( 'Expect:', 'X-API-Key: '.$config['key']));
curl_setopt($ch, CURLOPT_FOLLOWLOCATION, FALSE);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
$result=curl_exec($ch);
$code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);

if ($code != 201)
    die('Unable to create ticket: '.$result);

$ticket_id = (int) $result;
echo $ticket_id;
# Continue onward here if necessary. $ticket_id has the ID number of the
# newly-created ticket

?>
