(function () {
    function showDemo(demoIndex) {
        var demo = window.demos[demoIndex];
        var method = demo.method || 'GET';
        var url = window.location.protocol + "//" + window.location.host + demo.url;
        var body = demo.body ? $.trim($('#' + demo.body).html()) : '';

        $('#demos').find('button[data-demo=' + demoIndex + ']').addClass('demo-selected');
        $('#demos').find('button[data-demo!=' + demoIndex + ']').removeClass('demo-selected');
        $('#description').text(demo.description);
        $('#method').find('option:contains("' + method + '")').attr('selected', 'selected');
        $('#url').val(url);
        $('#requestbody').val(body);
    }

    function send() {
        var method = $('#method').find(':selected').text();
        var url = $('#url').val();
        var body = $('#requestbody').val();

        showResponse();
        $('#send').attr('disabled', 'disabled');

        $.ajax({
            type: method,
            url: url,
            data: body,
            dataType: 'text', // prevent jQuery from parsing the response
            accepts: 'application/json',
            contentType: 'application/json'
        }).done(function (data, textStatus, jqXHR) {
            showResponse(jqXHR.status, jqXHR.statusText, data, 'success');
        }).fail(function (jqXHR, textStatus, errorThrown) {
            showResponse(jqXHR.status, jqXHR.statusText, jqXHR.responseText, 'error');
        }).always(function () {
            $('#send').removeAttr('disabled');
        });
    }
    
    function showResponse(statusCode, statusText, body, responseType) {
        var statusText = statusCode ? (statusCode + ' ' + statusText) : '';
        var bodyText;

        try {
            bodyText = JSON.stringify(JSON.parse(body), null, 4);
        } catch (e) {
            bodyText = body || '';
        }

        $('#status').val(statusText);
        $('#responsebody').text(bodyText);

        if (responseType === 'success') {
            $('#status').addClass('status-success').removeClass('status-error');
        } else if (responseType === 'error') {
            $('#status').addClass('status-error').removeClass('status-success');
        } else {
            $('#status').removeClass('status-success').removeClass('status-error');
        }
    }

    $(function () {
        $.each(window.demos, function (i, demo) {
            var button = $('<button>').text(demo.name).attr('data-demo', i);
            button.appendTo('#demos');
        });

        $('#demos').on('click', 'button', function () {
            showDemo($(this).attr('data-demo'));
        });

        $('#send').on('click', send);
    });
})();