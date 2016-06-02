$(window).load(function(){
	$( document ).ready(function() {
		ReloadData()
		setInterval(function(){ReloadData()}, 1500);
	});


	function ReloadData(){

		$.get( "panelinfo.txt", function( panelinfo ) {
			var videoinfo = "Nothing";

$.ajax({
	url: '/variables.html',
	type: 'get',
	dataType: 'text',
	async: false,
	success: function( filename ) {

	if (filename.length > 0) {
	filename = filename.substring(0, filename.length-4);
	videoinfo = filename;
	}

	} 
});
		
			var panelinfo = panelinfo.replace('[No Video Information]', videoinfo );
			$('#PanelInfo').html( panelinfo );
		});
	}
});