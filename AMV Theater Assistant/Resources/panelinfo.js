$(window).load(function(){
	$( document ).ready(function() {
		ReloadData()
		setInterval(function(){ReloadData()}, 1500);
	});


	function ReloadData(){

		$.get( "panelinfo.html", function( panelinfo ) {
			var videoinfo = "Nothing";

$.ajax({
	url: '/variables.html',
	type: 'get',
	dataType: 'text',
	async: false,
	success: function( variables ) {

		if (variables.length > 0) {
		var filename = $(variables).text().split('\n');
		videoinfo = filename[9].substring(0,filename[9].length-4);
		}

	} 
});
		
			var panelinfo = panelinfo.replace('[No Video Information]', videoinfo );
			$('#PanelInfo').html( panelinfo );
		});
	}
});
