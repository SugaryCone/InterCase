// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var hcount = 0;

$(document).ready(function () {
	SHOW();
});
/*setInterval(function () {

}, 2000);
*/
async function SHOW() {
	$.getJSON('/api/History', function (data, textStatus, jqXHR) {
		var text = "";
		$.each(data, function (index, value) {
			text += '<div class="accordion-item" ><h3 class="accordion-header" id="heading' + index;
			text += `"><button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#accordionCollapse${index}" aria-expanded="false" aria-controls="accordionCollapse${index}">`;
			text += value['title'];
			text += ` </button></h3 ><div id="accordionCollapse${index}" class="accordion-collapse collapse" aria-labelledby="heading${index}" data-bs-parent="#accordionExample"><div class="accordion-body">`;
			text += value['content'];
			text += '</div></div ></div > ';

			
		});
		$("#accordionExample").html(text);

		$.get('/api/Notice', function (data, textStatus, jqXHR) {
			alert(data['content']);
			$("#dialog").dialog();

		});
	});




	$.getJSON('/api/Slide', function (data, textStatus, jqXHR) {
		hcount++;
		$("#Title").html(data['title']);
		$("#slidetext").html(data['content']);
		$("#slideforms").html(data['forms']);


	});
}

async function send_form() {
	var values = $("input[name='fslide']").map(function () { return $(this).is(':checked') ? $(this).val() : 0 }).get();

	alert(values);

}

async function NEXT() {

	var text = $("#fname").val();
	var values = $("input[name='fslide']").map(function () { return $(this).is(':checked') ? parseInt($(this).val()) : 0 }).get();
	$("#alertblock").empty();
	jQuery.ajax({
		url: "/api/Slide",
		type: "POST",
		data: JSON.stringify({ answer: values}),
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		success: function () {
			
		}
	});
}

async function Send() {

	$.get("/api/Slide", function (data, status) {
		//alert("Data: " + data + "\nStatus: " + status);
		$("#slideContent")
			.html(data);
	});
}


async function upd() {

	NEXT().then(function () {
		setTimeout(function () {
			SHOW();
		}, 500);
	})

}