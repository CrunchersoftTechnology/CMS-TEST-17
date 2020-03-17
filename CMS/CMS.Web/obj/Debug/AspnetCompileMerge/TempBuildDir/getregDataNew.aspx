﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getregDataNew.aspx.cs" Inherits=" CMS.Web.getregDataNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/jscript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.3/jquery.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {


            var sid = "";
            var cid = "";
            var status = "";
            var allowfees = "";
            var allowresult = "";
            var allowattendance = "";

            //getstudAttendanceData();
            //getstudResultData();
            //  getstudAttendanceData();
            // getstudResultData();
            /*   function chkstudReg() {
            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/checkOMRStudnet",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{username:'S1C1ST',password:'sandesh'}",
            dataType: "json",
            success: function (data) {

            cid = data.d[0].client_id;
            sid = data.d[0].stud_id;
            status = data.d[0].status;
            allowattendance = data.d[0].allow_attendance;
            allowfees = data.d[0].allow_fees;
            allowresult = data.d[0].allow_result;

            alert(sid);
            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }
            */

            var adate = "";
            var astatus = "";
            /*          function getstudAttendanceData() {
            alert('imin2nd method');
            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/getAttendanceDetails",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{dtFrom:'1/01/2017',dtTo:'20/01/2017',stud_id:'S1',client_id:'C1'}",
            dataType: "json",
            success: function (data) {
            alert(data.d.length);
            for (var i = 0; i < data.d.length; i++) {
            var adate = data.d[i].ADate;
            var astatus = data.d[i].Astatus;

            $("#dd").append("<h3>" + i + "-" + adate + "-" + astatus + "</h3");
            }
            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }


            getClassData();




            function getClassData() {

            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "http://cmsTest.crunchersoft.com/getregDataNew.aspx/getClassData",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{client_id:'C1'}",
            dataType: "json",
            success: function (data) {
            alert(data.d.length);
            for (var i = 0; i < data.d.length; i++) {
            var class_name = data.d[i].class_name;
            var head_name = data.d[i].head_name;
            var contact_no = data.d[i].contact_no;
            var email_id = data.d[i].email_id;
            var address = data.d[i].address;
            var website = data.d[i].website;
            //  var about_class = data.d[i].about_class;
            // alert(class_name + "-" + head_name + "_" + contact_no + "-" + email_id + "-" + address + "-" + website + "-" + about_class);
            }
            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }






            function getstudResultData() {

            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "http://cmsTest.crunchersoft.com/getregDataNew.aspx/getStudentResultData",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{stud_id:'" + sid + "',client_id:'" + cid + "'}",
            dataType: "json",
            success: function (data) {

            for (var i = 0; i < data.d.length; i++) {
            var test_id = data.d[i].test_id;
            var ob_mark = data.d[i].ob_mark;
            var tot_mark = data.d[i].tot_mark;
            var c_ans = data.d[i].c_ans;
            var inc_ans = data.d[i].inc_ans;
            var paper_name = data.d[i].paper_name;
            var t_rank = data.d[i].t_rank;
            var t_dt = data.d[i].t_dt;

            $("#dd").append("<h3>" + i + "-" + test_id + "-" + ob_mark + "-" + tot_mark + "-" + c_ans + "-" + inc_ans + "-" + paper_name + "-" + t_rank + " - " + t_dt + "</h3>");
            }
            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }
            */
            /*        getstudOvarallResultTopperData();

            function getstudOvarallResultTopperData() {
            alert('method');
            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/getStudentOverallResultTopperData",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{client_id:'C3'}",
            dataType: "json",
            success: function (data) {
            alert(data.d.length);
            for (var i = 0; i < data.d.length; i++) {
            var test_id = data.d[i].tt_perc;
            var ob_mark = data.d[i].tob_mark;
            var tot_mark = data.d[i].ttot_mark;
            var paper_name = data.d[i].ts_name;
            var t_rank = data.d[i].tt_rank;

            $("#dd").append("<h3>" + i + "-" + test_id + "-" + ob_mark + "-" + tot_mark + "-" + paper_name + " Rank- " + t_rank + "</h3>");
            }
            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }

            */
            /*
            getstudOvarallResultData();
            function getstudOvarallResultData() {
            alert('method');
            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/setOMRStudentPassword",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{sid:'S1',cid:'C1',pwd:'new1'}",
            dataType: "json",
            success: function (data) {
            alert("got it");

            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }


            */
            /*
            getstudFeeData();

            function getstudFeeData() {
            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/getCMSStudentFeesData",
            // url: "webservice1.asmx/CustomerRegistrationData",
            data: "{stud_id:'4735146b-9c99-4bdf-a2a6-29d5fe5bd015'}",
            dataType: "json",
            success: function (data) {
            console.log(data);
            //            var row = "";
            //            for (var i = 0; i < data.d.length; i++) {
            //            var total_fees = data.d[i].total_fee;
            //            var paid_fees = data.d[i].paid_fee;
            //            var rem_fees = data.d[i].rem_fee;
            //            alert(total_fees + "-" + paid_fees + "-" + rem_fees);
            //}

            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }

             
            getstudinfo();



            function getstudinfo() {
            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/getStudentInfo",
            data: "{stud_id:'S1',client_id:'C1'}",
            dataType: "json",
            success: function (data) {
            var row = "";
            for (var i = 0; i < data.d.length; i++) {

            var stud_name = data.d[i].stud_name;
            var class_name = data.d[i].className;
            var ac_year = data.d[i].ac_year;
            var stud_no = data.d[i].stud_mobNo;
            var parent_no = data.d[i].parent_mobNo;



            alert(stud_name + "-" + class_name + "-" + ac_year + "-" + stud_no + "-" + parent_no);
            }

            },
            error: function (result) {
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }


            try {
            getstudFeeHistoryData();
            }
            catch (r) {
            alert(r.Message);
            }
         
            getstudFeeHistoryData();

            function getstudFeeHistoryData() {

            $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "getregDataNew.aspx/checkOMRStudnet",
            data: "{username:'S4C3J', password:'4S4C3J', playerId:'sdfjh23hj38jsfh'}",
            dataType: "json",
            success: function (data) {
            console.log(data);
            //var row = "";
            //alert(data.d.length);
            //for (var i = 0; i < data.d.length; i++) {
            //    var receipt_no = data.d[i].receipt_no;
            //    var paid_fees = data.d[i].paid_fee;
            //    //  var paid_date = data.d[i].paid_date;
            //    alert(receipt_no + "-" + paid_fees);
            //}

            },
            error: function (result) {
            alert(result.responseText);
            $("#errorMsg").show();
            $("#errorMsg").html(result.responseText);
            }
            });
            }

            */

            //test();


            //            function test() {
            //                $.ajax({
            //                    type: "POST",
            //                    contentType: "application/json; charset=utf-8",
            //                    contentType: "application/json",
            //                    url: "getregDataNew.aspx/PostResultOfOnlineTest",
            //                    xhrFields: { withCredentials: true }, crossDomain: true,
            //                    dataType: "json",
            //                    data: "{testDetails:'1'}",
            //                    success: function (data) {
            //                        console.log(data);
            //                        //                        alert(data.length);
            //                        //                        for (var i = 0; i < data.length; i++) {
            //                        //                            alert(data);
            //                        //                        }
            //                    },
            //                    error: function (jqXHR, exception) {
            //                        var msg = '';
            //                        if (jqXHR.status === 0) {
            //                            msg = 'Please Verify Internet Connection';
            //                        } else if (jqXHR.status == 404) {
            //                            msg = 'Requested page not found. [404]';
            //                        } else if (jqXHR.status == 500) {
            //                            msg = 'Internal Server Error [500].';
            //                        } else if (exception === 'parsererror') {
            //                            msg = 'Requested JSON parse failed.';
            //                        } else if (exception === 'timeout') {
            //                            msg = 'Time out error.';
            //                        } else if (exception === 'abort') {
            //                            msg = 'Ajax request aborted.';
            //                        } else {
            //                            msg = 'Uncaught Error.\n' + jqXHR.responseText;
            //                        }
            //                        alert(msg);
            //                    }
            //                });
            //            }





//            sendingMessage();
//            function sendingMessage() {
//                //alert('method');
//                //var dob = new Date(Date.parse("1993-07-15", "yyyy-MM-dd"));
//                //console.log(dob);

//                //data: "{message:'Testing Message', username: 'crun-cstpune', password: '12312312', parentContact: '1212121212', sender: 'CTPUNE', lastName: 'Kandikatla', dob:'1993-2-2'}",
//                //data: "{message:'OTP for parent registration is.', username: 'crun-cstpune', password: '12312312', parentContact: '9767825033', sender: 'CTPUNE', lastName: 'gaikwad', dob:'1992-06-02'}",
//                //data: "{userId : 'e6e64263-a752-4f0b-9214-21ba95810e06'}",
//                //data: "{branchId : '4', classId : '4', batchId : '1'}",
//                // data: "{device_id:'98D-BF',app_code:'84'}",
//                //data: "{device_id:'98D-BF',app_code:'84', classname:'ScholarsKatta', studentName:'Priti Pali', mobileNumber:'9767825033', key:'QEQY-CTVC-BRRV-PREX', installationDate:'15-02-2018', expireDate:'15-02-2019'}",
//                //data: "{message:'OTP for parent registration is.', username: 'crun-cstpune', password: '12312312', studentContact: '9767825033', sender: 'CTPUNE', lastName: 'kale', dob:'1993-07-15', firstName: 'minal', emailId: 'pritispali1@gmail.com'}",
//                //data: "{branchId: '12' , classId: '2', selectedbatches:'1', notificationMaxId:'6'}",
//                //data: "{branchId:'"+branchId+"', classId:'"+classId+"', selectedbatches:'"+selectedBatches+"', notificationMaxId: '"+NotificationId+"', maxOfflineTestPaperId: '"+MaxOfflineTestPaperId+"', maxOfflineTestStudentMarksId: '"+MaxOfflineTestStudentMarksId+"', userId: '"+userId+"'}",
//                $.ajax({
//                    contentType: "application/json; charset=utf-8",
//                    xhrFields: { withCredentials: true },
//                    url: "getregData.aspx/sendingMessage",
//                    //                    data: "{userId: '86801852-1e2d-40f9-b919-b368ac500534'}",
//                    //                    data: "{branchId:'3', classId:'2', selectedbatches:'2', notificationMaxId: '19',pdfUploadId: '1', arrengeTestId: '19', maxOfflineTestPaperId: '11', maxOfflineTestStudentMarksId: '5', userId: '86801852-1e2d-40f9-b919-b368ac500534', selectedSubjects: '2,4', studentName: 'Priti Pali'}",
//                    //                    data: "{message:'OTP for parent registration is.', username: 'crun-cstpune', password: '12312312', studentContact: '9767825033', sender: 'CTPUNE', lastName: 'Pali', dob:'1992-06-10', firstName: 'priti', emailId: 'pritispali@gmail.com'}",
//                    data: "{message:'OTP for parent registration is.', username: 'crun-cstpune', password: '12312312', parentContact: '9767825033', sender: 'CTPUNE', lastName: 'pali', dob:'1992-06-10'}",
//                    async: true,
//                    dataType: 'json',   //you may use jsonp for cross origin request
//                    type: "POST",
//                    success: function (data) {
//                        console.log(data);
//                        alert("got it");

//                    },
//                    error: function (result) {
//                        $("#errorMsg").show();
//                        $("#errorMsg").html(result.responseText);
//                    }
//                });
//            }

            //                                    sendingMessage();
            //                                    function sendingMessage() {
            //                                        alert('method');
            //                                        $.ajax({
            //                                            contentType: "application/json; charset=utf-8",
            //                                            xhrFields: { withCredentials: true },
            //                                            url: "getregDataNew.aspx/getCMSAttendanceDataCount",
            //                                            data: "{SId: '3', ClassId: '2', selectedBatches:'1,5,6,8'}",
            //                                            async: true,
            //                                            dataType: 'json',   //you may us    e jsonp for cross origin request
            //                                            type: "POST",
            //                                            success: function (data) {
            //                                                console.log(data);
            //                                                alert("got it");

            //                                            },
            //                                            error: function (result) {
            //                                                $("#errorMsg").show();
            //                                                $("#errorMsg").html(result.responseText);
            //                                            }
            //                                        });
            //                                    }




        });



    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dd">
    </div>
    <div id="errorMsg" style="width: 90%; height: 90%; border: 1px solid red; background-color: yellow;
        overflow: scroll; z-index: 1500; position: fixed; display: none; margin: 2px;
        align: center;">
    </div>
    </form>
</body>
</html>



