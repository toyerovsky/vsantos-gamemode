var screenX = API.getScreenResolutionMantainRatio().Width;
var screenY = API.getScreenResolutionMantainRatio().Height;
var panelMinX = (screenX / 32);
var panelMinY = (screenY / 18);
var button = null;
var panel = null;
var image = null;
var notification = null;
var notifications = [];
var textnotification = null;
var textnotifications = [];
var padding = 10;
var selectedInput = null;
var tabIndex = [];
var tab = 0;
var menuElements = [];
var isReady = false;
var currentPage = 0;
var clickDelay = new Date().getTime();
var currentMenu = null;
API.onUpdate.connect(() => {
    drawNotification();
    drawTextNotification();
    if (!isReady) {
        return;
    }
    for (var i = 0; i < menuElements[currentPage].length; i++) {
        if (!isReady) {
            break;
        }
        menuElements[currentPage][i].draw();
        menuElements[currentPage][i].isHovered();
        menuElements[currentPage][i].isClicked();
    }
});
function createMenu(pages) {
    if (currentMenu !== null) {
        isReady = false;
        currentPage = 0;
        selectedInput = null;
        tabIndex = [];
        menuElements = [];
    }
    currentMenu = new Menu(pages);
    return currentMenu;
}
class Menu {
    constructor(pages) {
        if (Array.isArray(menuElements[0])) {
            return;
        }
        for (var i = 0; i < pages; i++) {
            let emptyArray = [];
            menuElements.push(emptyArray);
        }
        this._blur = false;
    }
    set Ready(value) {
        isReady = value;
        currentPage = 0;
    }
    get Ready() {
        return isReady;
    }
    set Blur(value) {
        this._blur = value;
        if (value) {
            API.callNative("_TRANSITION_TO_BLURRED", 3000);
        }
        else {
            API.callNative("_TRANSITION_FROM_BLURRED", 3000);
        }
    }
    get Blur() {
        return this._blur;
    }
    set Page(value) {
        currentPage = value;
    }
    get Page() {
        return currentPage;
    }
    DisableOverlays(value) {
        this._overlays = value;
        if (value) {
            API.setHudVisible(false);
            API.setChatVisible(false);
            API.setCanOpenChat(false);
            API.showCursor(true);
            return;
        }
        if (!value) {
            API.setHudVisible(true);
            API.setChatVisible(true);
            API.setCanOpenChat(true);
            API.showCursor(false);
            return;
        }
    }
    killMenu() {
        isReady = false;
        API.showCursor(false);
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.setCanOpenChat(true);
        API.callNative("_TRANSITION_FROM_BLURRED", 3000);
    }
    nextPage() {
        if (currentPage + 1 > menuElements.length - 1) {
            currentPage = 0;
            return;
        }
        currentPage++;
    }
    prevPage() {
        if (currentPage - 1 < 0) {
            currentPage = menuElements.length - 1;
            return;
        }
        currentPage--;
    }
    createPanel(page, xStart, yStart, xGridWidth, yGridHeight) {
        let newPanel = new Panel(page, xStart, yStart, xGridWidth, yGridHeight);
        menuElements[page].push(newPanel);
        return newPanel;
    }
}
class PlayerTextNotification {
    constructor(text) {
        let playerPos = API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 1));
        let point = API.worldToScreenMantainRatio(playerPos);
        this._xPos = Point.Round(point).X;
        this._yPos = Point.Round(point).Y;
        this._drawing = true;
        this._alpha = 255;
        this._text = text;
        this._increment = -1;
        this._lastUpdateAlpha = new Date().getTime();
        this._lastUpdateTextPosition = new Date().getTime();
        this._r = 0;
        this._g = 0;
        this._b = 0;
    }
    draw() {
        if (!this._drawing) {
            return;
        }
        if (new Date().getTime() > this._lastUpdateAlpha + 35) {
            this._lastUpdateAlpha = new Date().getTime();
            this._alpha -= 5;
        }
        if (new Date().getTime() > this._lastUpdateTextPosition + 100) {
            this._yPos -= 0.3;
        }
        API.drawText(this._text, this._xPos, this._yPos, 0.4, this._r, this._g, this._b, this._alpha, 4, 1, true, true, 500);
        if (this._alpha <= 0) {
            this.cleanUpNotification();
        }
    }
    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }
    cleanUpNotification() {
        this._drawing = false;
        textnotification = null;
    }
    returnType() {
        return "PlayerTextNotification";
    }
}
class ProgressBar {
    constructor(x, y, width, height, currentProgress) {
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX - 10;
        this._height = height * panelMinY - 10;
        this._currentProgress = currentProgress;
        this._r = 0;
        this._g = 0;
        this._b = 0;
        this._alpha = 255;
        this._drawText = true;
        API.sendChatMessage("Created");
    }
    draw() {
        API.drawRectangle(this._xPos + 5, this._yPos + 5, ((this._width / 100) * this._currentProgress), this._height, this._r, this._g, this._b, this._alpha);
        if (this._drawText) {
            API.drawText("" + Math.round(this._currentProgress), this._xPos + (((Math.round(this._width) / 100) * this._currentProgress) / 2), this._yPos, 0.5, 255, 255, 255, 255, 4, 1, false, true, 100);
        }
    }
    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }
    set Alpha(value) {
        this._alpha = value;
    }
    set DrawText(value) {
        this._drawText = value;
    }
    addProgress(value) {
        if (this._currentProgress + value > 100) {
            this._currentProgress = 100;
            return;
        }
        this._currentProgress += value;
    }
    subtractProgress(value) {
        if (this._currentProgress - value < 0) {
            this._currentProgress = 0;
            return;
        }
        this._currentProgress -= value;
    }
    setProgressAmount(value) {
        if (value >= 100) {
            this._currentProgress = 100;
            return;
        }
        if (value <= 0) {
            this._currentProgress = 0;
            return;
        }
        this._currentProgress = value;
        return;
    }
    returnProgressAmount() {
        return this._currentProgress;
    }
    returnType() {
        return "ProgressBar";
    }
}
class Notification {
    constructor(text, displayTime) {
        this._currentPosX = 26 * panelMinX;
        this._currentPosY = screenY;
        this._targetX = 26 * panelMinX;
        this._targetY = 15 * panelMinY;
        this._width = panelMinX * 5;
        this._height = panelMinY * 3;
        this._text = text;
        this._r = 255;
        this._g = 165;
        this._b = 0;
        this._offset = 0;
        this._textScale = 0.5;
        this._lastUpdateTime = new Date().getTime();
        this._alpha = 255;
        this._displayTime = displayTime;
        this._incrementer = 0;
        this._sound = true;
    }
    draw() {
        if (notification !== this) {
            return;
        }
        if (this._sound) {
            this._sound = false;
            API.playSoundFrontEnd("GOLF_NEW_RECORD", "HUD_AWARDS");
        }
        API.drawRectangle(this._currentPosX, this._currentPosY - 5, this._width, 5, this._r, this._g, this._b, this._alpha - 30);
        API.drawRectangle(this._currentPosX, this._currentPosY, this._width, this._height, 0, 0, 0, this._alpha - 30);
        API.drawText(this._text, this._offset + this._currentPosX + (Math.round(this._width) / 2), this._currentPosY + (this._height / 4), this._textScale, 255, 255, 255, this._alpha, 4, 1, false, false, Math.round(this._width) - padding);
        this.animate();
    }
    animate() {
        if (this._currentPosY <= this._targetY) {
            this._currentPosY = this._targetY;
            if (new Date().getTime() > this._lastUpdateTime + this._displayTime) {
                this.fade();
                return;
            }
            return;
        }
        this._lastUpdateTime = new Date().getTime();
        if (this._currentPosY <= this._targetY + (this._height / 6)) {
            this._currentPosY -= 3;
            return;
        }
        else {
            this._currentPosY -= 5;
            return;
        }
    }
    fade() {
        if (this._alpha <= 0) {
            this.cleanUpNotification();
            return;
        }
        this._alpha -= 5;
        return;
    }
    cleanUpNotification() {
        notification = null;
    }
    setText(value) {
        this._text = value;
    }
    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }
    setTextScale(value) {
        this._textScale = value;
    }
    isHovered() {
        return;
    }
    isClicked() {
        return;
    }
    returnType() {
        return "Notification";
    }
}
class TextElement {
    constructor(text, x, y, width, height, line) {
        this._xPos = x;
        this._yPos = y + (panelMinY * line);
        this._width = width;
        this._height = height;
        this._text = text;
        this._fontScale = 0.6;
        this._centered = false;
        this._centeredVertically = false;
        this._font = 4;
        this._fontR = 255;
        this._fontG = 255;
        this._fontB = 255;
        this._fontAlpha = 255;
        this._hoverTextAlpha = 255;
        this._hoverTextR = 255;
        this._hoverTextG = 255;
        this._hoverTextB = 255;
        this._offset = 0;
        this._padding = 10;
        this._hovered = false;
        this._shadow = false;
        this._outline = false;
    }
    draw() {
        if (this._centered && this._centeredVertically) {
            this.drawAsCenteredAll();
            return;
        }
        if (this._centered) {
            this.drawAsCentered();
            return;
        }
        if (this._centeredVertically) {
            this.drawAsCenteredVertically();
            return;
        }
        this.drawAsNormal();
    }
    set Text(value) {
        this._text = value;
    }
    set Hovered(value) {
        this._hovered = value;
    }
    get Hovered() {
        return this._hovered;
    }
    Color(r, g, b, a) {
        this._fontR = r;
        this._fontG = g;
        this._fontB = b;
        this._fontAlpha = a;
    }
    HoverColor(r, g, b, a) {
        this._hoverTextR = r;
        this._hoverTextG = g;
        this._hoverTextB = b;
        this._hoverTextAlpha = a;
    }
    set R(value) {
        this._fontR = value;
    }
    get R() {
        return this._fontR;
    }
    set G(value) {
        this._fontG = value;
    }
    get G() {
        return this._fontG;
    }
    set B(value) {
        this._fontB = value;
    }
    get B() {
        return this._fontB;
    }
    set Alpha(value) {
        this._fontAlpha = value;
    }
    get Alpha() {
        return this._fontAlpha;
    }
    set HoverAlpha(value) {
        this._hoverTextAlpha = value;
    }
    get HoverAlpha() {
        return this._hoverTextAlpha;
    }
    set HoverR(value) {
        this._hoverTextR = value;
    }
    get HoverR() {
        return this._hoverTextR;
    }
    set HoverG(value) {
        this._hoverTextG = value;
    }
    get HoverG() {
        return this._hoverTextG;
    }
    set HoverB(value) {
        this._hoverTextB = value;
    }
    get HoverB() {
        return this._hoverTextB;
    }
    set Font(value) {
        this._font = value;
    }
    get Font() {
        return this._font;
    }
    set FontScale(value) {
        this._fontScale = value;
    }
    get FontScale() {
        return this._fontScale;
    }
    set VerticalCentered(value) {
        this._centeredVertically = value;
    }
    get VerticalCentered() {
        return this._centeredVertically;
    }
    set Centered(value) {
        this._centered = value;
    }
    get Centered() {
        return this._centered;
    }
    set Offset(value) {
        this._offset = value;
    }
    drawAsCenteredAll() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 20, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 20, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
    }
    drawAsCenteredVertically() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos, this._yPos + (this._height / 2) - 20, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width));
            return;
        }
        API.drawText(this._text, this._offset + this._xPos, this._yPos + (this._height / 2) - 20, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width));
    }
    drawAsCentered() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._padding + this._yPos, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + (Math.round(this._width) / 2), this._padding + this._yPos, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 1, this._shadow, this._outline, Math.round(this._width));
    }
    drawAsNormal() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + this._padding, this._yPos + this._padding, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width) - this._padding);
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + this._padding, this._yPos + this._padding, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 0, this._shadow, this._outline, Math.round(this._width) - this._padding);
    }
}
class Panel {
    constructor(page, x, y, width, height) {
        this._page = page;
        this._padding = 10;
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX;
        this._height = height * panelMinY;
        this._alpha = 225;
        this._header = false;
        this._offset = 0;
        this._r = 0;
        this._g = 0;
        this._b = 0;
        this._textLines = [];
        this._inputPanels = [];
        this._progressBars = [];
        this._currentLine = 0;
        this._shadow = false;
        this._outline = false;
        this._tooltip = null;
        this._hovered = false;
        this._hoverTime = 0;
        this._hoverR = 0;
        this._hoverG = 0;
        this._hoverB = 0;
        this._hoverAlpha = 200;
        this._backgroundImage = null;
        this._backgroundImagePadding = 0;
        this._function = null;
        this._functionArgs = [];
        this._functionAudioLib = "Click";
        this._functionAudioName = "DLC_HEIST_HACKING_SNAKE_SOUNDS";
        this._hoverAudioLib = "Cycle_Item";
        this._hoverAudioName = "DLC_Dmod_Prop_Editor_Sounds";
        this._hoverAudio = true;
        this._functionClickAudio = true;
        this._hoverable = false;
        this._line = 0;
    }
    draw() {
        if (this._page !== currentPage) {
            return;
        }
        this.drawRectangles();
        if (!isReady) {
            return;
        }
        if (this._textLines.length > 0) {
            for (var i = 0; i < this._textLines.length; i++) {
                this._textLines[i].draw();
            }
        }
        if (this._inputPanels.length > 0) {
            for (var i = 0; i < this._inputPanels.length; i++) {
                this._inputPanels[i].draw();
            }
        }
        if (this._progressBars.length > 0) {
            for (var i = 0; i < this._progressBars.length; i++) {
                this._progressBars[i].draw();
            }
        }
    }
    drawRectangles() {
        if (this._backgroundImage !== null) {
            this.drawBackgroundImage();
            return;
        }
        if (this._hovered) {
            API.drawRectangle(this._xPos, this._yPos, this._width, this._height, this._hoverR, this._hoverG, this._hoverB, this._hoverAlpha);
            if (this._header) {
                API.drawRectangle(this._xPos, this._yPos + this._height - 5, this._width, 5, 255, 255, 255, 50);
            }
            return;
        }
        API.drawRectangle(this._xPos, this._yPos, this._width, this._height, this._r, this._g, this._b, this._alpha);
        if (this._header) {
            API.drawRectangle(this._xPos, this._yPos + this._height - 5, this._width, 5, 255, 255, 255, 50);
        }
    }
    drawBackgroundImage() {
        if (this._backgroundImagePadding > 1) {
            API.dxDrawTexture(this._backgroundImage, new Point(this._xPos + this._backgroundImagePadding, this._yPos + this._backgroundImagePadding), new Size(this._width - (this._backgroundImagePadding * 2), this._height - (this._backgroundImagePadding * 2)), 0);
            API.drawRectangle(this._xPos, this._yPos, this._width, this._height, this._r, this._g, this._b, this._alpha);
            return;
        }
        API.dxDrawTexture(this._backgroundImage, new Point(this._xPos, this._yPos), new Size(this._width, this._height), 0);
    }
    set Function(value) {
        this._function = value;
    }
    addFunctionArgs(value) {
        this._functionArgs = value;
    }
    set HoverAudioLib(value) {
        this._hoverAudioLib = value;
    }
    get HoverAudioLib() {
        return this._hoverAudioLib;
    }
    set HoverAudioName(value) {
        this._hoverAudioName = value;
    }
    get HoverAudioName() {
        return this._hoverAudioName;
    }
    set FunctionAudioLib(value) {
        this._functionAudioLib = value;
    }
    get FunctionAudioLib() {
        return this._functionAudioLib;
    }
    set FunctionAudioName(value) {
        this._functionAudioName = value;
    }
    get FunctionAudioName() {
        return this._functionAudioName;
    }
    set FunctionAudio(value) {
        this._functionClickAudio = value;
    }
    get FunctionAudio() {
        return this._functionClickAudio;
    }
    set MainAlpha(value) {
        this._alpha = value;
    }
    get MainAlpha() {
        return this._alpha;
    }
    set MainBackgroundImagePadding(value) {
        this._backgroundImagePadding = value;
    }
    get MainBackgroundImagePadding() {
        return this._backgroundImagePadding;
    }
    set MainBackgroundImage(value) {
        this._backgroundImage = value;
    }
    get MainBackgroundImage() {
        return this._backgroundImage;
    }
    set MainColorR(value) {
        this._r = value;
    }
    get MainColorR() {
        return this._r;
    }
    set MainColorG(value) {
        this._g = value;
    }
    get MainColorG() {
        return this._g;
    }
    set MainColorB(value) {
        this._b = value;
    }
    get MainColorB() {
        return this._b;
    }
    MainBackgroundColor(r, g, b, alpha) {
        this._r = r;
        this._g = g;
        this._b = b;
        this._alpha = alpha;
    }
    HoverBackgroundColor(r, g, b, alpha) {
        this._hoverR = r;
        this._hoverG = g;
        this._hoverB = b;
        this._hoverAlpha = alpha;
    }
    set Hoverable(value) {
        this._hoverable = value;
    }
    get Hoverable() {
        return this._hoverable;
    }
    set HoverAlpha(value) {
        this._hoverAlpha = value;
    }
    get HoverAlpha() {
        return this._hoverAlpha;
    }
    set HoverR(value) {
        this._hoverR = value;
    }
    get HoverR() {
        return this._hoverR;
    }
    set HoverG(value) {
        this._hoverG = value;
    }
    get HoverG() {
        return this._hoverG;
    }
    set HoverB(value) {
        this._hoverB = value;
    }
    get HoverB() {
        return this._hoverB;
    }
    set FontOutline(value) {
        this._outline = value;
    }
    get FontOutline() {
        return this._outline;
    }
    set FontShadow(value) {
        this._shadow = value;
    }
    get FontShadow() {
        return this._shadow;
    }
    set Tooltip(value) {
        this._tooltip = value;
    }
    get Tooltip() {
        return this._tooltip;
    }
    set Header(value) {
        this._header = value;
    }
    get Header() {
        return this._header;
    }
    set Offset(value) {
        this._offset = value;
    }
    get Offset() {
        return this._offset;
    }
    addText(value) {
        let textElement = new TextElement(value, this._xPos, this._yPos, this._width, this._height, this._line);
        this._textLines.push(textElement);
        this._line += 1;
        return textElement;
    }
    addInput(x, y, width, height) {
        let inputPanel = new InputPanel(this._page, (x * panelMinX) + this._xPos, (y * panelMinY) + this._yPos, width, height);
        this._inputPanels.push(inputPanel);
        return inputPanel;
    }
    addProgressBar(x, y, width, height, currentProgress) {
        let progressBar = new ProgressBar((x * panelMinX) + this._xPos, (y * panelMinY) + this._yPos, width, height, currentProgress);
        this._progressBars.push(progressBar);
        return progressBar;
    }
    isHovered() {
        if (!API.isCursorShown()) {
            return;
        }
        if (!this._hoverable) {
            return;
        }
        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < this._yPos + this._height) {
            if (!this._hovered) {
                this._hovered = true;
                this.setTextHoverState(true);
                if (this._hoverAudio) {
                    API.playSoundFrontEnd(this._hoverAudioLib, this._hoverAudioName);
                }
            }
            this._hoverTime += 1;
            if (this._hoverTime > 50) {
                if (this._tooltip === null) {
                    return;
                }
                if (this._tooltip.length > 1) {
                    API.drawText(this._tooltip, cursorPos.X + 25, cursorPos.Y, 0.4, 255, 255, 255, 255, 4, 0, true, true, 200);
                }
            }
            return;
        }
        this._hovered = false;
        this._hoverTime = 0;
        this.setTextHoverState(false);
    }
    isClicked() {
        if (!API.isCursorShown()) {
            return;
        }
        if (this._function === null) {
            return;
        }
        if (API.isControlJustPressed(237)) {
            let cursorPos = API.getCursorPositionMantainRatio();
            if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < this._yPos + this._height) {
                if (new Date().getTime() < clickDelay + 200) {
                    return;
                }
                clickDelay = new Date().getTime();
                if (this._functionClickAudio) {
                    API.playSoundFrontEnd(this._functionAudioLib, this._functionAudioName);
                }
                if (this._functionArgs !== null || this._functionArgs.length > 1) {
                    this._function(this._functionArgs);
                }
                else {
                    this._function();
                }
                return;
            }
        }
    }
    setTextHoverState(value) {
        for (var i = 0; i < this._textLines.length; i++) {
            this._textLines[i].Hovered = value;
        }
    }
    returnType() {
        return "Panel";
    }
}
class InputPanel {
    constructor(page, x, y, width, height) {
        this._xPos = x;
        this._yPos = y;
        this._width = width * panelMinX;
        this._height = height * panelMinY;
        this._protected = false;
        this._input = "";
        this._hovered = false;
        this._selected = false;
        this._numeric = false;
        this._isError = false;
        this._isTransparent = false;
        this._r = 255;
        this._g = 255;
        this._b = 255;
        this._alpha = 100;
        this._hoverR = 255;
        this._hoverG = 255;
        this._hoverB = 255;
        this._hoverAlpha = 125;
        this._selectR = 255;
        this._selectG = 255;
        this._selectB = 255;
        this._inputTextR = 0;
        this._inputTextG = 0;
        this._inputTextB = 0;
        this._inputTextAlpha = 255;
        this._selectAlpha = 125;
        this._inputAudioLib = "Click";
        this._inputAudioName = "DLC_HEIST_HACKING_SNAKE_SOUNDS";
        this._page = page;
        this._inputTextScale = 0.45;
        tabIndex.push(this);
    }
    set isError(value) {
        this._isError = value;
    }
    set Selected(value) {
        this._selected = value;
        if (value) {
            selectedInput = this;
        }
        else {
            selectedInput = null;
        }
    }
    get Selected() {
        return this._selected;
    }
    set HoverR(value) {
        this._hoverR = value;
    }
    get HoverR() {
        return this._hoverR;
    }
    set HoverG(value) {
        this._hoverG = value;
    }
    get HoverG() {
        return this._hoverG;
    }
    set HoverB(value) {
        this._hoverB = value;
    }
    get HoverB() {
        return this._hoverB;
    }
    set HoverAlpha(value) {
        this._hoverAlpha = value;
    }
    get HoverAlpha() {
        return this._hoverAlpha;
    }
    MainBackgroundColor(r, g, b, alpha) {
        this._r = r;
        this._g = g;
        this._b = b;
        this._alpha = alpha;
    }
    HoverBackgroundColor(r, g, b, alpha) {
        this._hoverR = r;
        this._hoverG = g;
        this._hoverB = b;
        this._hoverAlpha = alpha;
    }
    SelectBackgroundColor(r, g, b, alpha) {
        this._selectR = r;
        this._selectG = g;
        this._selectB = b;
        this._selectAlpha = alpha;
    }
    set R(value) {
        this._r = value;
    }
    get R() {
        return this._r;
    }
    set G(value) {
        this._g = value;
    }
    get G() {
        return this._g;
    }
    set B(value) {
        this._b = value;
    }
    get B() {
        return this._b;
    }
    set Alpha(value) {
        this._alpha = value;
    }
    get Alpha() {
        return this._alpha;
    }
    set SelectR(value) {
        this._selectR = value;
    }
    get SelectR() {
        return this._selectR;
    }
    set SelectG(value) {
        this._selectG = value;
    }
    get SelectG() {
        return this._selectG;
    }
    set SelectB(value) {
        this._selectB = value;
    }
    get SelectB() {
        return this._selectB;
    }
    set SelectAlpha(value) {
        this._selectAlpha = value;
    }
    get SelectAlpha() {
        return this._selectAlpha;
    }
    InputTextColor(r, g, b, alpha) {
        this._inputTextR = r;
        this._inputTextG = g;
        this._inputTextB = b;
        this._inputTextAlpha = alpha;
    }
    set TextR(value) {
        this._inputTextR = value;
    }
    get TextR() {
        return this._inputTextR;
    }
    set TextG(value) {
        this._inputTextG = value;
    }
    get TextG() {
        return this._inputTextG;
    }
    set TextB(value) {
        this._inputTextB = value;
    }
    get InputB() {
        return this._inputTextB;
    }
    set InputAlpha(value) {
        this._inputTextAlpha = value;
    }
    get InputAlpha() {
        return this._inputTextAlpha;
    }
    set InputTextScale(value) {
        this._inputTextScale = value;
    }
    get InputTextScale() {
        return this._inputTextScale;
    }
    set Input(value) {
        this._input = value;
    }
    get Input() {
        return this._input;
    }
    removeFromInput() {
        this._input = this._input.substring(0, this._input.length - 1);
    }
    set NumericOnly(value) {
        this._numeric = value;
    }
    get NumericOnly() {
        return this._numeric;
    }
    set Protected(value) {
        this._protected = value;
    }
    draw() {
        if (this._selected) {
            this.selectedDraw();
        }
        if (this._hovered) {
            this.hoveredDraw();
        }
        if (!this._hovered && !this._selected) {
            this.normalDraw();
        }
        this.isHovered();
        this.isClicked();
    }
    normalDraw() {
        if (this._isError) {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 0, 0, 100);
        }
        else {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, this._r, this._g, this._b, this._alpha);
        }
        if (this._protected) {
            if (this._input.length < 1) {
                return;
            }
            API.drawText("*".repeat(this._input.length), this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        else {
            API.drawText(this._input, this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
    }
    selectedDraw() {
        API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, this._selectR, this._selectG, this._selectB, this._selectAlpha);
        if (this._protected) {
            if (this._input.length < 1) {
                return;
            }
            API.drawText("*".repeat(this._input.length), this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        else {
            API.drawText(this._input, this._xPos + (Math.round(this._width) / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * Math.round(this._width)));
        }
        return;
    }
    hoveredDraw() {
        if (this._isError) {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 0, 0, 100);
        }
        else {
            API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, this._hoverR, this._hoverG, this._hoverB, this._hoverAlpha);
        }
        if (this._protected) {
            if (this._input.length < 1) {
                return;
            }
            API.drawText("*".repeat(this._input.length), this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * this._width));
        }
        else {
            API.drawText(this._input, this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, this._inputTextScale, this._inputTextR, this._inputTextG, this._inputTextB, this._inputTextAlpha, 4, 1, false, false, (panelMinX * this._width));
        }
    }
    isHovered() {
        if (!API.isCursorShown()) {
            return;
        }
        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < (this._yPos + this._height)) {
            if (this._selected) {
                this._hovered = false;
                return;
            }
            this._hovered = true;
        }
        else {
            this._hovered = false;
        }
    }
    isClicked() {
        if (API.isControlJustPressed(237)) {
            if (new Date().getTime() < clickDelay + 200) {
                return;
            }
            let cursorPos = API.getCursorPositionMantainRatio();
            if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < (this._yPos + this._height)) {
                if (!this._selected) {
                    API.playSoundFrontEnd(this._inputAudioLib, this._inputAudioName);
                    this._selected = true;
                }
                selectedInput = this;
            }
            else {
                this._selected = false;
            }
        }
    }
    addToInput(text) {
        if (this._input.length > 2147483647) {
            return;
        }
        if (!this._numeric) {
            this._input += text;
            return this._input;
        }
        else {
            if (Number.isInteger(+text)) {
                this._input += text;
                return this._input;
            }
        }
    }
    returnType() {
        return "InputPanel";
    }
}
function drawTextNotification() {
    if (textnotification !== null) {
        textnotification.draw();
        return;
    }
    if (textnotifications.length <= 0) {
        return;
    }
    textnotification = textnotifications.shift();
    return;
}
function drawNotification() {
    if (notification !== null) {
        notification.draw();
        return;
    }
    if (notifications.length <= 0) {
        return;
    }
    notification = notifications.shift();
    return;
}
function createNotification(page, text, displayTime) {
    let notify = new Notification(text, displayTime);
    notifications.push(notify);
    return notify;
}
function createPlayerTextNotification(text) {
    let notify = new PlayerTextNotification(text);
    textnotifications.push(notify);
    return notify;
}
function setPage(value) {
    currentPage = value;
}
function killMenu() {
    isReady = false;
    currentPage = -1;
    selectedInput = null;
    tabIndex = [];
    API.showCursor(false);
    API.setHudVisible(true);
    API.setChatVisible(true);
    API.setCanOpenChat(true);
    API.callNative("_TRANSITION_FROM_BLURRED", 3000);
    menuElements = [];
}
API.onKeyDown.connect(function (sender, e) {
    if (!isReady) {
        return;
    }
    if (e.KeyCode == Keys.Tab) {
        if (tabIndex[0].Selected) {
            tabIndex[0].Selected = false;
        }
        else {
            tabIndex[0].Selected = true;
            return;
        }
        let removeItem = tabIndex.shift();
        tabIndex.push(removeItem);
        tabIndex[0].Selected = true;
    }
    if (selectedInput === null) {
        return;
    }
    if (e.KeyCode === Keys.Back) {
        selectedInput.removeFromInput();
        return;
    }
    let shiftOn = false;
    if (e.Shift) {
        shiftOn = true;
    }
    let keypress = "";
    switch (e.KeyCode) {
        case Keys.Space:
            keypress = " ";
            break;
        case Keys.A:
            keypress = "a";
            if (shiftOn) {
                keypress = "A";
            }
            break;
        case Keys.B:
            keypress = "b";
            if (shiftOn) {
                keypress = "B";
            }
            break;
        case Keys.C:
            keypress = "c";
            if (shiftOn) {
                keypress = "C";
            }
            break;
        case Keys.D:
            keypress = "d";
            if (shiftOn) {
                keypress = "D";
            }
            break;
        case Keys.E:
            keypress = "e";
            if (shiftOn) {
                keypress = "E";
            }
            break;
        case Keys.F:
            keypress = "f";
            if (shiftOn) {
                keypress = "F";
            }
            break;
        case Keys.G:
            keypress = "g";
            if (shiftOn) {
                keypress = "G";
            }
            break;
        case Keys.H:
            keypress = "h";
            if (shiftOn) {
                keypress = "H";
            }
            break;
        case Keys.I:
            keypress = "i";
            if (shiftOn) {
                keypress = "I";
            }
            break;
        case Keys.J:
            keypress = "j";
            if (shiftOn) {
                keypress = "J";
            }
            break;
        case Keys.K:
            keypress = "k";
            if (shiftOn) {
                keypress = "K";
            }
            break;
        case Keys.L:
            keypress = "l";
            if (shiftOn) {
                keypress = "L";
            }
            break;
        case Keys.M:
            keypress = "m";
            if (shiftOn) {
                keypress = "M";
            }
            break;
        case Keys.N:
            keypress = "n";
            if (shiftOn) {
                keypress = "N";
            }
            break;
        case Keys.O:
            keypress = "o";
            if (shiftOn) {
                keypress = "O";
            }
            break;
        case Keys.P:
            keypress = "p";
            if (shiftOn) {
                keypress = "P";
            }
            break;
        case Keys.Q:
            keypress = "q";
            if (shiftOn) {
                keypress = "Q";
            }
            break;
        case Keys.R:
            keypress = "r";
            if (shiftOn) {
                keypress = "R";
            }
            break;
        case Keys.S:
            keypress = "s";
            if (shiftOn) {
                keypress = "S";
            }
            break;
        case Keys.T:
            keypress = "t";
            if (shiftOn) {
                keypress = "T";
            }
            break;
        case Keys.U:
            keypress = "u";
            if (shiftOn) {
                keypress = "U";
            }
            break;
        case Keys.V:
            keypress = "v";
            if (shiftOn) {
                keypress = "V";
            }
            break;
        case Keys.W:
            keypress = "w";
            if (shiftOn) {
                keypress = "W";
            }
            break;
        case Keys.X:
            keypress = "x";
            if (shiftOn) {
                keypress = "X";
            }
            break;
        case Keys.Y:
            keypress = "y";
            if (shiftOn) {
                keypress = "Y";
            }
            break;
        case Keys.Z:
            keypress = "z";
            if (shiftOn) {
                keypress = "Z";
            }
            break;
        case Keys.D0:
            keypress = "0";
            if (shiftOn) {
                keypress = ")";
            }
            break;
        case Keys.D1:
            keypress = "1";
            if (shiftOn) {
                keypress = "!";
            }
            break;
        case Keys.D2:
            keypress = "2";
            if (shiftOn) {
                keypress = "@";
            }
            break;
        case Keys.D3:
            keypress = "3";
            if (shiftOn) {
                keypress = "#";
            }
            break;
        case Keys.D4:
            keypress = "4";
            if (shiftOn) {
                keypress = "$";
            }
            break;
        case Keys.D5:
            keypress = "5";
            if (shiftOn) {
                keypress = "%";
            }
            break;
        case Keys.D6:
            keypress = "6";
            if (shiftOn) {
                keypress = "^";
            }
            break;
        case Keys.D7:
            keypress = "7";
            if (shiftOn) {
                keypress = "&";
            }
            break;
        case Keys.D8:
            keypress = "8";
            if (shiftOn) {
                keypress = "*";
            }
            break;
        case Keys.D9:
            keypress = "9";
            if (shiftOn) {
                keypress = "(";
            }
            break;
        case Keys.OemMinus:
            keypress = "-";
            if (shiftOn) {
                keypress = "_";
            }
            break;
        case Keys.Oemplus:
            keypress = "=";
            if (shiftOn) {
                keypress = "+";
            }
            break;
        case Keys.OemQuestion:
            keypress = "/";
            if (shiftOn) {
                keypress = "?";
            }
            break;
        case Keys.Oemcomma:
            keypress = ",";
            if (shiftOn) {
                keypress = "<";
            }
            break;
        case Keys.OemPeriod:
            keypress = ".";
            if (shiftOn) {
                keypress = ">";
            }
            break;
        case Keys.OemSemicolon:
            keypress = ";";
            if (shiftOn) {
                keypress = ":";
            }
            break;
        case Keys.OemOpenBrackets:
            keypress = "[";
            if (shiftOn) {
                keypress = "{";
            }
            break;
        case Keys.OemCloseBrackets:
            keypress = "]";
            if (shiftOn) {
                keypress = "}";
            }
            break;
        case Keys.NumPad0:
            keypress = "0";
            break;
        case Keys.NumPad1:
            keypress = "1";
            break;
        case Keys.NumPad2:
            keypress = "2";
            break;
        case Keys.NumPad3:
            keypress = "3";
            break;
        case Keys.NumPad4:
            keypress = "4";
            break;
        case Keys.NumPad5:
            keypress = "5";
            break;
        case Keys.NumPad6:
            keypress = "6";
            break;
        case Keys.NumPad7:
            keypress = "7";
            break;
        case Keys.NumPad8:
            keypress = "8";
            break;
        case Keys.NumPad9:
            keypress = "9";
            break;
    }
    if (keypress === "") {
        return;
    }
    if (keypress.length > 0) {
        if (selectedInput === null) {
            return;
        }
        selectedInput.addToInput(keypress);
    }
    else {
        return;
    }
});
var loginMenu;
var loginUsername;
var loginPassword;
var regUsername;
var regPassword;
var regConfirmPassword;
function menuLoginPanel() {
    loginMenu = createMenu(4);
    let panel;
    let inputPanel;
    let textElement;
    panel = loginMenu.createPanel(0, 12, 4, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Warunki uzytkowania");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Font = 1;
    panel = loginMenu.createPanel(0, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Akceptujac niniejszy kontrakt zgadzasz sie z warunkami uzytkowania obowiazujacymi na serwerze. Pliki jakie otrzymaleś moga sluzyc tylko i wylacznie do gry na serwerze V-Santos.pl manipulacja nimi jest zabroniona. Zlamanie warunków kontraktu bedzie skutkowala permamentna banicja konta.");
    textElement.Color(255, 255, 255, 255);
    textElement.FontScale = 0.4;
    textElement.Font = 4;
    textElement.Centered = false;
    panel = loginMenu.createPanel(0, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Function = loginMenu.nextPage;
    panel.Tooltip = "Zaakceptuj warunki";
    textElement = panel.addText("Accept");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    panel = loginMenu.createPanel(1, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = false;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    panel = loginMenu.createPanel(1, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Nowe konto?";
    panel.Function = loginMenu.nextPage;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText(">");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);
    panel = loginMenu.createPanel(1, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Twój e-mail:");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Twoje haslo:");
    textElement.Color(255, 255, 255, 255);
    loginUsername = panel.addInput(0, 1, 8, 1);
    loginPassword = panel.addInput(0, 3, 8, 1);
    loginPassword.Protected = true;
    panel = loginMenu.createPanel(1, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Tooltip = "Przejdź do logowania";
    textElement = panel.addText("Login");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    panel = loginMenu.createPanel(2, 12, 4, 7, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Rejestracja");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = false;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Offset = 18;
    panel = loginMenu.createPanel(2, 19, 4, 1, 1);
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Tooltip = "Istniejace konto?";
    panel.Function = loginMenu.prevPage;
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Header = true;
    textElement = panel.addText("<");
    textElement.Color(255, 255, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.HoverColor(0, 180, 255, 255);
    panel = loginMenu.createPanel(2, 12, 5, 8, 7);
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("E-mail");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Haslo");
    textElement.Color(255, 255, 255, 255);
    panel.addText("");
    textElement = panel.addText("Password Again");
    textElement.Color(255, 255, 255, 255);
    regUsername = panel.addInput(0, 1, 8, 1);
    regPassword = panel.addInput(0, 3, 8, 1);
    regPassword.Protected = true;
    regConfirmPassword = panel.addInput(0, 5, 8, 1);
    regConfirmPassword.Protected = true;
    panel = loginMenu.createPanel(2, 12, 12, 8, 1);
    panel.MainBackgroundColor(0, 0, 0, 160);
    panel.HoverBackgroundColor(25, 25, 25, 160);
    panel.Hoverable = true;
    panel.Tooltip = "Zarejestruj sie";
    textElement = panel.addText("Register");
    textElement.Color(255, 255, 255, 255);
    textElement.HoverColor(0, 180, 255, 255);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    loginMenu.Blur = true;
    loginMenu.DisableOverlays(true);
    loginMenu.Ready = true;
}
API.onServerEventTrigger.connect((eventName, ...args) => {
    if (eventName == "ShowLoginCef") {
        if (args[0]) {
            API.showCursor(true);
            menuLoginPanel();
        }
        else {
        }
    }
});
//# sourceMappingURL=Compiled.js.map