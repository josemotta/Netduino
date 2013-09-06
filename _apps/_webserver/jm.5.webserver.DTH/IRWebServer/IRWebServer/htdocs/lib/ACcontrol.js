$(document).ready(function() {
	status();
	getInTemp();
	getSettings();
	
	$('#power').click(function() {
		send("/?ir=0");
	});
	
	$('#onTime').click(function() {
		var url = "/?on=" + $('#onMin').val();
		if($('#onMin').val() != "") {
			send(url);
		}
	});
	
	$('#offTime').click(function() {
		var url = "/?off=" + $('#offMin').val();
		if($('#offMin').val() != "") {
			send(url);
		}
	});
	
	$('#onTemp').click(function() {
		var url = "/?onT=" + $('#onDegrees').val();
		if($('#onDegrees').val() != "") {
			send(url);
		}
	});
	
	$('#offTemp').click(function() {
		var url = "/?offT=" + $('#offDegrees').val();
		if($('#offDegrees').val() != "") {
			send(url);
		}
	})
	
	$('#refresh').click(function() {
		status();
		getInTemp();
		getSettings();
	});
});
	
function send(address) {
	$.when(
		$.ajax({
			url: address,
			success: function(data){
				if (!(data === ""))
				{
					status();
					getInTemp();
					getSettings();
					alert(data);
				}
			}
		})
	).done(function(data, textStatus, jqXHR){
		if (data === "")
		{
			send(address);
		}
	});
}

function status() {
	$.when(
		$.ajax({
			url: '/?status=get',
			success: function(data){
				if (!(data === ""))
				{
					$('#status').html(data + ".");
				}
			}
		})
	).done(function(data, textStatus, jqXHR){
		if (data === "")
		{
			status(address);
		}
	});
}

function getInTemp() {
	$.when(
		$.ajax({
			url: "/?getTemp=get",
			cache: false,
			success: function(data){
				if(data != '') {
					try {
						data = $.parseJSON(data);
					} catch (e) {
						return;
					}
					$(".temp").html("<p>Temperature: " + data.temp + "F Humidity: " + data.hum + "%</p><p>Heat index: " + data.index + "F</p>");
				}
			}
		})
	).done(function(data, textStatus, jqXHR){
		if(data == '') {
			getInTemp();
		} else {
			try {
				$.parseJSON(data);
			} catch (e) {
				getInTemp();
			}
		}
	});
}

function getSettings() {
	$.when(
		$.ajax({
			url: "/?getSettings=get",
			cache: false,
			success: function(data){
				if(data != '') {
					try {
						data = $.parseJSON(data);
					} catch (e) {
						return;
					}
					$("#onT").html(data.high);
					$("#offT").html(data.low);
				}
			}
		})
	).done(function(data, textStatus, jqXHR){
		if(data == '') {
			getInTemp();
		} else {
			try {
				$.parseJSON(data);
			} catch (e) {
				getInTemp();
			}
		}
	});
}