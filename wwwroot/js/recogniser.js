function hexToBase64(str) {
    return btoa(String.fromCharCode.apply(null, str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" ")));
}
var readfile = function () {
    var image = $("#imagefile");
    var reader = new FileReader();
    reader.onload = function () {

        var arrayBuffer = this.result,
		array = new Uint8Array(arrayBuffer),
		binaryString = String.fromCharCode.apply(null, array);
        binaryString = btoa(binaryString)

    }
    reader.readAsArrayBuffer(image[0].files[0]);

};


$("#recognise").click(function (e) {
	
    console.log("Recognise pressed");
	readfile();

});