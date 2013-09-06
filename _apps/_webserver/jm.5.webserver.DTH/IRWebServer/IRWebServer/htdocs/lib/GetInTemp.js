$(document).ready(function() {
	getInTemp();
});

function getInTemp() {
	$.when(
		$.ajax({
			url: "/auth.htm?getTemp=get",
			cache: false,
			success: function(data){
				if(data != "") {
					try {
						data  = $.parseJSON(data);
					} catch (e) {
						return;
					}
					$(".temp").html("<h3>Temperature: " + data.temp + "F Humidity: " + data.hum + "%</h3><p>Heat index: " + data.index + "F</p>");
				}
			}
		})
	).done(function(data, textStatus, jqXHR){
		if(data === "") {
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