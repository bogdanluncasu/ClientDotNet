function hexToBase64(str) {
    return btoa(String.fromCharCode.apply(null, str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" ")));
}
var readfile = function () {
    var image = $("#imagefile");
    var reader = new FileReader();
    reader.onload = function () {

        var arrayBuffer = this.result,
		array = new Uint8Array(arrayBuffer),
        binaryString = "";
        for (i=0;i<array.length;i+=3)
            binaryString += String.fromCharCode.apply(null, array.slice(i, i + 3));
		binaryString = btoa(binaryString);
        $.ajax({
            url: "https://objectrecognize.herokuapp.com/image/issport",
            type: "POST",
            data: JSON.stringify({ "image": binaryString }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                imagesource = $("#imagesource");
                labelinfo = $("#labelinfo");
                labelinfo.html(data["label"].toUpperCase());
                if (data["label"] == "sport") {
                    labelinfo.css('color', 'green');
                    
                    $.ajax({
                        url: "https://objectrecognize.herokuapp.com/image/objectrecognition",
                        type: "POST",
                        data: JSON.stringify({ "image": binaryString }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            image=data["image"]
                            imagesource.attr('src', 'data:image/jpeg;base64,' + btoa(image));
                            objectdiv = $("#objects");

                            objectdiv.html("");
                            element = $('<ul></ul>');
                            objects = data["objects"]
                            objects.sort(function(a, b) { 
                                return b.matches - a.matches;
                            })

                            var maxmatches = 1;
                            if (objects.length > 0)
                                maxmatches = objects[0]['matches']

                            for (var obj in objects) {
                                objname = objects[obj]['object'];
                                color = objects[obj]['color'];
                                matches = objects[obj]['matches'];
                                color = color[2] + ',' + color[1] + ',' + color[0];

                                element.append($('<li></li>')
                                    .attr({style:'color:rgb('+color+')' })
                                    .text(objname+' - '+parseInt(((matches/maxmatches)*100)))
                                );
                            }
                            objectdiv.append(element);
                            console.log(data["objects"]);
                        }
                    })

                } else {
                    labelinfo.css('color', 'red');
                    imagesource.attr('src', 'data:image/jpeg;base64,' + binaryString);
                }
                console.log(data["label"]);
            }
        });



    }
    reader.readAsArrayBuffer(image[0].files[0]);

};


$("#recognise").click(function (e) {
	
    console.log("Recognise pressed");
	readfile();

});