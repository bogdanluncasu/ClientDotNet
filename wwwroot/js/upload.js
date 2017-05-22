function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

var readfile = function (token) {
    var file = $("#archive");
    var label = $("#label").val();
    var reader = new FileReader();
    reader.onload = function () {

        var arrayBuffer = this.result,
		array = new Uint8Array(arrayBuffer),
		binaryString = String.fromCharCode.apply(null, array);
        binaryString = btoa(binaryString)
        $.ajax({
            url: "/Objects/Add",
            type: "POST",
            data: JSON.stringify({
                label : label,
                "archive": binaryString
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#info").text(data);
            },
            error: function (data) {
                $("#info").text(data.responseText);
            }
        
        });

    }
    reader.readAsArrayBuffer(file[0].files[0]);

};

$("#add_object").click(function () {
    console.log("Archive processing...");
    readfile();
});