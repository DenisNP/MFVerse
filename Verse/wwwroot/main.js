let A;

function initializeClient(token, appReference) {
    const initPhrase = 'запусти формулы по алгебре';

    log('token got: ' + token);

    A = token !== ''
        ? assistant.createSmartappDebugger({ getState: function(){}, token: token, initPhrase: initPhrase })
        : assistant.createAssistant(function(){});

    A.on('start', function() {
        log('assistant started');
    });
    
    A.on('data', function (command){
        if (command.type === 'insets') {
            let b = command.insets && command.insets.bottom;
            if (b) {
                const root = document.documentElement;
                if (b > 200) b = 144; // TODO await fix
                root.style.setProperty('--bottom-inset', b + 'px');
            }
        }
        appReference.invokeMethodAsync('CommandGot', JSON.stringify(command));
    });
}

function sendData(action, payload, enableCallback) {
    A.sendData({ action: { action_id: action, payload: payload } }, enableCallback ? null : function (){});
}

function close() {
    A && A.close();
}

function log(s) {
    console && console.log(s);
}