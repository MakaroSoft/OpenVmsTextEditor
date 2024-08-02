// no jquery and no css. Only use vanilla javascript

// create the namesoace
if (MakaroSoft === undefined) {
    var MakaroSoft = {};
}

// define the class
MakaroSoft.CommonNotification = function () {

    let _guid = "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );

    let _notifyIcon = `<svg style="vertical-align: middle" width="24px" height="24px" viewBox="0 0 24 24" fill="#fff" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="2.5"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path fill-rule="evenodd" clip-rule="evenodd" d="M11.5 0.999992C10.9477 0.999992 10.5 1.44771 10.5 1.99999V2.99999H9.99998C7.23864 2.99999 4.99998 5.23824 4.99998 7.99975V11C4.99998 11.7377 4.76718 12.5722 4.39739 13.4148C4.03164 14.2482 3.55875 15.0294 3.14142 15.6439C2.38188 16.7624 2.85215 18.5301 4.40564 18.8103C5.42144 18.9935 6.85701 19.2115 8.54656 19.3527C8.54454 19.4015 8.54352 19.4506 8.54352 19.5C8.54352 21.433 10.1105 23 12.0435 23C13.9765 23 15.5435 21.433 15.5435 19.5C15.5435 19.4482 15.5424 19.3966 15.5402 19.3453C17.1921 19.204 18.596 18.9903 19.5943 18.8103C21.1478 18.5301 21.6181 16.7624 20.8586 15.6439C20.4412 15.0294 19.9683 14.2482 19.6026 13.4148C19.2328 12.5722 19 11.7377 19 11V7.99975C19 5.23824 16.7613 2.99999 14 2.99999H13.5V1.99999C13.5 1.44771 13.0523 0.999992 12.5 0.999992H11.5ZM12 19.5C12.5113 19.5 13.0122 19.4898 13.4997 19.4715C13.5076 20.2758 12.8541 20.9565 12.0435 20.9565C11.2347 20.9565 10.5803 20.2778 10.5872 19.4746C11.0473 19.491 11.5191 19.5 12 19.5ZM9.99998 4.99999C8.34305 4.99999 6.99998 6.34297 6.99998 7.99975V11C6.99998 12.1234 6.65547 13.2463 6.22878 14.2186C5.79804 15.2 5.25528 16.0911 4.79599 16.7675C4.78578 16.7825 4.78102 16.7969 4.77941 16.8113C4.77797 16.8242 4.77919 16.8362 4.78167 16.8458C6.3644 17.1303 9.00044 17.5 12 17.5C14.9995 17.5 17.6356 17.1303 19.2183 16.8458C19.2208 16.8362 19.222 16.8242 19.2206 16.8113C19.2189 16.7969 19.2142 16.7825 19.204 16.7675C18.7447 16.0911 18.2019 15.2 17.7712 14.2186C17.3445 13.2463 17 12.1234 17 11V7.99975C17 6.34297 15.6569 4.99999 14 4.99999H9.99998Z" fill="#fff"></path> <path fill-rule="evenodd" clip-rule="evenodd" d="M16.0299 0.757457C16.1622 0.228068 16.7146 -0.102469 17.2437 0.0301341C17.3131 0.0476089 17.3789 0.0669732 17.4916 0.104886C17.6295 0.151258 17.8183 0.221479 18.0424 0.322098C18.4894 0.522794 19.0851 0.848127 19.6982 1.35306C20.9431 2.37831 22.2161 4.1113 22.495 6.9005C22.55 7.45005 22.149 7.94009 21.5995 7.99504C21.05 8.05 20.5599 7.64905 20.505 7.09951C20.2839 4.88869 19.3068 3.62168 18.4268 2.89692C17.9774 2.52686 17.5418 2.28969 17.2232 2.14664C17.0645 2.07538 16.9369 2.02841 16.8541 2.00057C16.8201 1.98913 16.7859 1.97833 16.7513 1.96858C16.2192 1.83203 15.8964 1.2912 16.0299 0.757457Z" fill="#fff"></path> <path fill-rule="evenodd" clip-rule="evenodd" d="M7.97014 0.757457C7.83619 0.221662 7.29326 -0.104099 6.75746 0.0298498C6.68765 0.0473468 6.62176 0.066766 6.5084 0.104885C6.37051 0.151257 6.1817 0.221478 5.9576 0.322097C5.51059 0.522793 4.91493 0.848125 4.30179 1.35306C3.05685 2.37831 1.78388 4.1113 1.50496 6.90049C1.45001 7.45003 1.85095 7.94008 2.40049 7.99503C2.95004 8.04998 3.44008 7.64904 3.49504 7.0995C3.71612 4.88869 4.69315 3.62168 5.57321 2.89692C6.02257 2.52686 6.45815 2.28969 6.77678 2.14664C6.93548 2.07538 7.06308 2.02841 7.14589 2.00057C7.17991 1.98913 7.21413 1.97833 7.24867 1.96858C7.78081 1.83203 8.10358 1.2912 7.97014 0.757457Z" fill="#fff"></path> </g></svg>`;
    let _warningIcon = `<svg style="vertical-align: middle" width="24px" height="24px" fill="#fff" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="2.5"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"><path d="M15.35 12.81 9 2.08a1.22 1.22 0 0 0-2 0L.65 12.81a1.14 1.14 0 0 0 1 1.69h12.66a1.14 1.14 0 0 0 1.04-1.69zm-13.66.55L8 2.64l6.31 10.72z"></path><path d="M7.32 5.45h1.25V10H7.32z"></path><ellipse cx="7.95" cy="11.9" rx=".67" ry=".7"></ellipse></g></svg>`;

    //https://www.svgrepo.com/svg/438508/error?edit=true
    let _errorIcon = `<svg style="vertical-align: middle" width="24px" height="24px" fill="#fff" viewBox="-3.5 0 19 19" xmlns="http://www.w3.org/2000/svg" class="cf-icon-svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"><path d="M11.383 13.644A1.03 1.03 0 0 1 9.928 15.1L6 11.172 2.072 15.1a1.03 1.03 0 1 1-1.455-1.456l3.928-3.928L.617 5.79a1.03 1.03 0 1 1 1.455-1.456L6 8.261l3.928-3.928a1.03 1.03 0 0 1 1.455 1.456L7.455 9.716z"></path></g></svg>`;

    let _successIcon = `<svg style="vertical-align: middle" width="24px" height="24px" fill="#fff" viewBox="0 0 64 64" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" xml:space="preserve" xmlns:serif="http://www.serif.com/" style="fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:2;"><g id="SVGRepo_bgCarrier" stroke-width="2.5"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <rect id="Icons" x="-512" y="-64" width="1280" height="800" style="fill:none;"></rect> <g id="Icons1" serif:id="Icons"> <g id="Strike"> </g> <g id="H1"> </g> <g id="H2"> </g> <g id="H3"> </g> <g id="list-ul"> </g> <g id="hamburger-1"> </g> <g id="hamburger-2"> </g> <g id="list-ol"> </g> <g id="list-task"> </g> <g id="trash"> </g> <g id="vertical-menu"> </g> <g id="horizontal-menu"> </g> <g id="sidebar-2"> </g> <g id="Pen"> </g> <g id="Pen1" serif:id="Pen"> </g> <g id="clock"> </g> <g id="external-link"> </g> <g id="hr"> </g> <path id="success" d="M56.103,16.824l-33.296,33.297l-14.781,-14.78l2.767,-2.767l11.952,11.952l30.53,-30.53c0.943,0.943 1.886,1.886 2.828,2.828Z" style="fill-rule:nonzero;"></path> <g id="info"> </g> <g id="warning"> </g> <g id="plus-circle"> </g> <g id="minus-circle"> </g> <g id="vue"> </g> <g id="cog"> </g> <g id="logo"> </g> <g id="radio-check"> </g> <g id="eye-slash"> </g> <g id="eye"> </g> <g id="toggle-off"> </g> <g id="shredder"> </g> <g id="spinner--loading--dots-" serif:id="spinner [loading, dots]"> </g> <g id="react"> </g> <g id="check-selected"> </g> <g id="turn-off"> </g> <g id="code-block"> </g> <g id="user"> </g> <g id="coffee-bean"> </g> <g id="coffee-beans"> <g id="coffee-bean1" serif:id="coffee-bean"> </g> </g> <g id="coffee-bean-filled"> </g> <g id="coffee-beans-filled"> <g id="coffee-bean2" serif:id="coffee-bean"> </g> </g> <g id="clipboard"> </g> <g id="clipboard-paste"> </g> <g id="clipboard-copy"> </g> <g id="Layer1"> </g> </g> </g></svg>`;
    let _infoIcon = `<svg style="vertical-align: middle" width="24px" height="24px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="2.5"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <g clip-path="url(#clip0_429_11160)"> <circle cx="12" cy="11.9999" r="9" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"></circle> <rect x="12" y="8" width="0.01" height="0.01" stroke="#fff" stroke-width="3.75" stroke-linejoin="round"></rect> <path d="M12 12V16" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"></path> </g> <defs> <clipPath id="clip0_429_11160"> <rect width="24" height="24" fill="#fff"></rect> </clipPath> </defs> </g></svg>`;

    let _xIcon = `<svg style="vertical-align: middle" class="_close_me_" xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='#000'><path  class="_close_me_" d='M.293.293a1 1 0 011.414 0L8 6.586 14.293.293a1 1 0 111.414 1.414L9.414 8l6.293 6.293a1 1 0 01-1.414 1.414L8 9.414l-6.293 6.293a1 1 0 01-1.414-1.414L6.586 8 .293 1.707a1 1 0 010-1.414z'></svg>`;


    // yes styles instead of css. I Have dozens of places to implement this and I want to make it as simple as possible
    let _alertStyle = `background-color: white;
border-style: solid;
border-width: 1px 1px 1px 50px;
margin-bottom: 5px;
padding: 16px 16px;
border-radius: 4px;
position: relative`;

    let _buttonStyle = `box-sizing: content-box;
width: 1em;
height: 1em;
color: #000;
border: 0;
border-radius: 4px;
opacity: .5;
position: absolute;
top: 0;
right: 0;
z-index: 2;
background-color: white;
padding: 16px 16px`;

    let _spanStyle = `color: white;
margin-left: -50px;
margin-right: 25px`;



    let div = document.createElement("div");
    div.setAttribute("style", "width: 40%; position: fixed; top: 10px; right: 10px; z-index: 2147483647;");
    div.setAttribute("id", _guid);

    document.body.prepend(div);

    div.addEventListener("click",
        function (event) {
            if (event.target.getAttribute("class") === "_close_me_")
            {
                event.target.closest("div").remove();
            }
        });

    this.show = function (message, messageType, delay) {
        delay = typeof delay !== 'undefined' ? delay : 0;
        if (!messageType) { messageType = 'info'; }


        let icon = "";
        let borderColor = "";
        switch (messageType) {
            case "notify":
                icon = _notifyIcon;
                borderColor = "#287DA1";
                break;
            case "warning":
                icon = _warningIcon;
                borderColor = "#ffc107";
                break;
            case "error":
                icon = _errorIcon;
                borderColor = "#dc3545";
                break;
            case "success":
                icon = _successIcon;
                borderColor = "#198754";
                break;
            case "info":
            default:
                icon = _infoIcon;
                borderColor = "#287DA1";
                break;
        }

        let myDiv = document.createElement("div");
        myDiv.setAttribute("style", `border-color: ${borderColor}; ${_alertStyle}`);

        let mySpan = document.createElement("span");
        mySpan.setAttribute("style", _spanStyle);
        mySpan.innerHTML += icon;

        let myButton = document.createElement("button");
        myButton.setAttribute("style", _buttonStyle);
        myButton.setAttribute("type", "button");
        myButton.setAttribute("class", "_close_me_");
        myButton.innerHTML += _xIcon;

        myDiv.appendChild(mySpan);
        myDiv.appendChild(myButton);


        myDiv.innerHTML += message;

        document.getElementById(_guid).append(myDiv);

        if (delay > 0) {
            window.setTimeout(function () {
                myDiv.remove();
            }, delay);
        }
    }

    // obsolete - use clearAll instead
    this.hide = function () {
        document.getElementById(_guid).innerHTML = "";
    }

    this.clearAll = function () {
        document.getElementById(_guid).innerHTML = "";
    }
}