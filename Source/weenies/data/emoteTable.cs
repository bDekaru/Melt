using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Melt
{
    public enum eVendorEmoteType
    {
        none = 0,
        onOpenShop = 1,
        onCloseShop = 2,
        onBuyItemFromPlayer = 3,
        onSellItemToPlayer = 4,
        onUnknownVendorEmoteType = 5
    }

    public enum eEmoteTriggerCategory
    {
        none = 0,
        onRefuseItem = 1,
        onShop = 2,
        onDeath = 3,
        onPortal = 4,
        onIdle = 5,
        onReceiveItem = 6,
        onTalk = 7,
        onUse = 8,
        onUnknownEmoteCategory1 = 9,
        onPickup = 10,
        onUnknownEmoteCategory2 = 11,
        onQuestFlagCheckSuccess = 12,
        onQuestFlagCheckFailure = 13,
        onTaunt = 14,
        onHitPointsThreshold = 15,
        onKill = 16,
        onAttack = 17,
        onCallReinforcements = 18,//maybe
        onRetreat = 19,
        onSpellLanded = 20,
        onSpellResisted = 21,
        OnFlagCheckSuccess = 22,
        OnFlagCheckFailure = 23,
        onUnknownEmoteCategory3 = 24,
        onUnknownEmoteCategory4 = 25,
        onUnknownEmoteCategory5 = 26,
        onEventControllerRunning = 27,
        onEventControllerNotRunning = 28,
        OnFlagCheckFailure2 = 29,
        OnFlagCheckSuccess2 = 30,
        OnFlagCheckFailure3 = 31, //assumed, not confirmed
        OnFlagCheckSuccess3 = 32
    }

    public enum eEmoteActionType
    {
        none = 0,
        textEmote = 1,
        giveExp = 2,
        giveItem = 3,
        moveToHomePosition = 4,
        performEmote = 5,
        moveTo = 6,
        showParticleEffects = 7,
        textSay = 8,
        playSound = 9,
        textTell = 10,
        rotate = 11,
        turnToPlayer = 12,
        textDirect = 13,
        castSpell = 14,
        useActivationTarget = 15,
        textWorldBroadcast = 16,
        textLocalBroadcast = 17,
        textPrivate = 18,
        castSpellInstant = 19,
        updateQuestFlag = 20,
        checkQuestFlag = 21,
        setQuestFlag = 22,
        startEvent = 23,
        stopEvent = 24,
        textUnknown1 = 25,
        textAdmin = 26,
        teachSpell = 27,
        giveExpSkill = 28,
        increaseSkill = 29,
        checkQuestCompletionCounter = 30,
        removeQuestFlag = 31,
        decrementQuestCounter = 32,
        incrementQuestCounter = 33,
        setTitle = 34,
        checkBoolStatAndSetFlag = 35,
        checkIntStatAndSetFlag = 36,
        checkFloatStatAndSetFlag = 37,
        checkStringStatAndSetFlag = 38,
        checkAttributeAndSetFlag = 39,
        checkUnbuffedAttributeAndSetFlag = 40,
        checkVitalAndSetFlag = 41,
        checkRawVitalAndSetFlag = 42,
        checkSkillAndSetFlag = 43,
        checkUnbuffedSkillAndSetFlag = 44,
        checkIsSkillTrainedAndSetFlag = 45,
        checkIsSkillSpecializedAndSetFlag = 46,
        giveSkillCredit = 47,
        giveVitae = 48,
        giveExpLevelProportional = 49,
        giveSkillExpLevelProportional = 50,
        startEventController = 51,
        playerPerformEmote = 52,
        setIntStat = 53,
        increaseIntStat = 54,
        decreaseIntStat = 55,
        giveMutatedLoot = 56,
        resetHomePosition = 57,
        checkFellowshipQuestFlag = 58,
        unknownEmoteType = 59,
        updateFellowshipQuestFlag = 60,
        setFellowshipQuestFlag = 61,
        giveExpFellowship = 62,
        setLifestonePosition = 63,
        textFellowshipTell = 64,
        textFellowshipBroadcast = 65,
        lockFellowship = 66,
        gotoEmote = 67
    }

    public interface iEmoteTableAction
    {
        void writeRaw(StreamWriter outputStream);
        void writeJson(StreamWriter outputStream, string tab, bool isFirst);
    }

    public struct sEmoteTableActionBasicType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;

        public sEmoteTableActionBasicType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionMotionType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public int motion;

        public sEmoteTableActionMotionType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            motion = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32(motion, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "motion", motion, subEntriesTab, false, true, 10);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionMsgType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;

        public sEmoteTableActionMsgType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionAmountType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public int amount;

        public sEmoteTableActionAmountType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            amount = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32(amount, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "amount", amount, subEntriesTab, false, true, 10);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionMsgAmountType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public int amount;

        public sEmoteTableActionMsgAmountType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            amount = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32(amount, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "amount", amount, subEntriesTab, false, true, 10);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionPScriptType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public ePScriptType pscript;

        public sEmoteTableActionPScriptType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            pscript = (ePScriptType)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32((int)pscript, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "pscript", (int)pscript, subEntriesTab, false, true, 9);
            Utils.writeJson(outputStream, "_comment2", pscript.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionSoundType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public eSoundType sound;

        public sEmoteTableActionSoundType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            sound = (eSoundType)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32((int)sound, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "sound", (int)sound, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "_comment2", sound.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionFrameType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public sFrame frame;

        public sEmoteTableActionFrameType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            frame = new sFrame(inputFile);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            frame.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write("\n{0}}}", entriesTab);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            frame.writeRaw(outputStream);
        }
    }

    struct sEmoteTableActionMPositionType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public sPosition mPosition;

        public sEmoteTableActionMPositionType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            mPosition = new sPosition(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            mPosition.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            mPosition.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionCProfType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public sCprof cprof;

        public sEmoteTableActionCProfType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            cprof = new sCprof(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            cprof.writeRaw(outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            cprof.writeJson(outputStream, subEntriesTab, false);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionAmountHeroxp64Type : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public Int64 amount64;
        public Int64 heroxp64;

        public sEmoteTableActionAmountHeroxp64Type(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            amount64 = Utils.readInt64(inputFile);
            heroxp64 = Utils.readInt64(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt64(amount64, outputStream);
            Utils.writeInt64(heroxp64, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "amount64", amount64, subEntriesTab, false, true, 8);
            Utils.writeJson(outputStream, "heroxp64", heroxp64, subEntriesTab, false, true, 8);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionAmountStatType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public int amount;
        public eSkills stat;

        public sEmoteTableActionAmountStatType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            amount = Utils.readInt32(inputFile);
            stat = (eSkills)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32(amount, outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "amount", amount, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionSpellIdType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public int spellid;

        public sEmoteTableActionSpellIdType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            spellid = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32((int)spellid, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "spellid", spellid, subEntriesTab, false, true, 9);
            Utils.writeJson(outputStream, "_comment2", SpellInfo.getSpellName(spellid), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionMinMaxPercent64Type : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public double percent;
        public Int64 min64;
        public Int64 max64;
        public int display;

        public sEmoteTableActionMinMaxPercent64Type(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            percent = Utils.readDouble(inputFile);
            min64 = Utils.readInt64(inputFile);
            max64 = Utils.readInt64(inputFile);
            display = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeDouble(percent, outputStream);
            Utils.writeInt64(min64, outputStream);
            Utils.writeInt64(max64, outputStream);
            Utils.writeInt32(display, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "percent", percent, subEntriesTab, false, true, 9);
            Utils.writeJson(outputStream, "min64", min64, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "max64", max64, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "display", display, subEntriesTab, false, true, 9);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionMinMaxPercentSkillType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public eSkills stat;
        public double percent;
        public int min;
        public int max;
        public int display;

        public sEmoteTableActionMinMaxPercentSkillType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            stat = (eSkills)Utils.readInt32(inputFile);
            percent = Utils.readDouble(inputFile);
            min = Utils.readInt32(inputFile);
            max = Utils.readInt32(inputFile);
            display = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32((int)stat, outputStream);
            Utils.writeDouble(percent, outputStream);
            Utils.writeInt32(min, outputStream);
            Utils.writeInt32(max, outputStream);
            Utils.writeInt32(display, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "percent", percent, subEntriesTab, false, true, 9);
            Utils.writeJson(outputStream, "min", min, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "max", max, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "stat", display, subEntriesTab, false, true, 14);
            Utils.writeJson(outputStream, "display", display, subEntriesTab, false, true, 9);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionBoolStatType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public eBoolStat stat;

        public sEmoteTableActionBoolStatType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            stat = (eBoolStat)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionIntStatType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public int min;
        public int max;
        public eIntStat stat;

        public sEmoteTableActionIntStatType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            min = Utils.readInt32(inputFile);
            max = Utils.readInt32(inputFile);
            stat = (eIntStat)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32(min, outputStream);
            Utils.writeInt32(max, outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "min", min, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "max", max, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionFloatStatType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public Double fmin;
        public Double fmax;
        public eFloatStat stat;

        public sEmoteTableActionFloatStatType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            fmin = Utils.readDouble(inputFile);
            fmax = Utils.readDouble(inputFile);
            stat = (eFloatStat)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeDouble(fmin, outputStream);
            Utils.writeDouble(fmax, outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "fmin", fmin, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "fmax", fmax, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionStringStatType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public string teststring;
        public eStringStat stat;

        public sEmoteTableActionStringStatType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            teststring = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            stat = (eStringStat)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(teststring), outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "teststring", teststring, subEntriesTab, false, true, 2);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionMinMaxAttributeType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public int min;
        public int max;
        public eAttributes stat;

        public sEmoteTableActionMinMaxAttributeType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            min = Utils.readInt32(inputFile);
            max = Utils.readInt32(inputFile);
            stat = (eAttributes)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32(min, outputStream);
            Utils.writeInt32(max, outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "min", min, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "max", max, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionMinMaxVitalsType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public int min;
        public int max;
        public eVitals stat;

        public sEmoteTableActionMinMaxVitalsType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            min = Utils.readInt32(inputFile);
            max = Utils.readInt32(inputFile);
            stat = (eVitals)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32(min, outputStream);
            Utils.writeInt32(max, outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "min", min, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "max", max, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionMinMaxSkillsType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public int min;
        public int max;
        public eSkills stat;

        public sEmoteTableActionMinMaxSkillsType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            min = Utils.readInt32(inputFile);
            max = Utils.readInt32(inputFile);
            stat = (eSkills)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32(min, outputStream);
            Utils.writeInt32(max, outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "min", min, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "max", max, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    struct sEmoteTableActionMsgSkillsType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public eSkills stat;

        public sEmoteTableActionMsgSkillsType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            stat = (eSkills)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32((int)stat, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionMsgMinMax : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public string msg;
        public int min;
        public int max;

        public sEmoteTableActionMsgMinMax(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
            min = Utils.readInt32(inputFile);
            max = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(msg), outputStream);
            Utils.writeInt32(min, outputStream);
            Utils.writeInt32(max, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "msg", msg, subEntriesTab, false, true, 7);
            Utils.writeJson(outputStream, "min", min, subEntriesTab, false, true, 13);
            Utils.writeJson(outputStream, "max", max, subEntriesTab, false, true, 13);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionIntStatAmountType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public eIntStat stat;
        public int amount;

        public sEmoteTableActionIntStatAmountType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            stat = (eIntStat)Utils.readInt32(inputFile);
            amount = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32((int)stat, outputStream);
            Utils.writeInt32(amount, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "stat", (int)stat, subEntriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", stat.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "amount", amount, subEntriesTab, false, true, 10);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTableActionTreasureType : iEmoteTableAction
    {
        public eEmoteActionType type;
        public Single delay;
        public Single extent;
        public int wealth_rating;
        public int treasure_class;
        public eTreasureType treasure_type;

        public sEmoteTableActionTreasureType(StreamReader inputFile, eEmoteActionType actionType)
        {
            type = actionType;
            delay = Utils.readSingle(inputFile);
            extent = Utils.readSingle(inputFile);
            wealth_rating = Utils.readInt32(inputFile);
            treasure_class = Utils.readInt32(inputFile);
            treasure_type = (eTreasureType)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)type, outputStream);
            Utils.writeSingle(delay, outputStream);
            Utils.writeSingle(extent, outputStream);
            Utils.writeInt32(wealth_rating, outputStream);
            Utils.writeInt32(treasure_class, outputStream);
            Utils.writeInt32((int)treasure_type, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string subEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            Utils.writeJson(outputStream, "type", (int)type, subEntriesTab, true, true, 12);
            Utils.writeJson(outputStream, "_comment", type.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "delay", delay, subEntriesTab, false, true, 11);
            Utils.writeJson(outputStream, "extent", extent, subEntriesTab, false, true, 10);
            Utils.writeJson(outputStream, "wealth_rating", wealth_rating, subEntriesTab, false, true, 3);
            Utils.writeJson(outputStream, "treasure_class", treasure_class, subEntriesTab, false, true, 2);
            Utils.writeJson(outputStream, "treasure_type", (int)treasure_type, subEntriesTab, false, true, 3);
            Utils.writeJson(outputStream, "_comment2", treasure_type.ToString(), "    ", false, false, 0);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public interface iEmoteTableTrigger
    {
        void writeRaw(StreamWriter outputStream);
        void writeJson(StreamWriter outputStream, string tab, bool isFirst);
    }

    public struct sEmoteTableTriggerEmpty : iEmoteTableTrigger
    {
        public void writeRaw(StreamWriter outputStream)
        {
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
        }
    }

    public struct sEmoteTableTriggerBasicType : iEmoteTableTrigger
    {
        public eEmoteTriggerCategory category;
        public Single probability;

        public sEmoteTableTriggerBasicType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
        {
            category = triggerCategory;
            probability = Utils.readSingle(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)category, outputStream);
            Utils.writeSingle(probability, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
        }
    }

    public struct sEmoteTableTriggerStyleSubstyleType : iEmoteTableTrigger
    {
        public eEmoteTriggerCategory category;
        public Single probability;
        public uint style;
        public uint substyle;

        public sEmoteTableTriggerStyleSubstyleType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
        {
            category = triggerCategory;
            probability = Utils.readSingle(inputFile);
            style = Utils.readUInt32(inputFile);
            substyle = Utils.readUInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)category, outputStream);
            Utils.writeSingle(probability, outputStream);
            Utils.writeUInt32(style, outputStream);
            Utils.writeUInt32(substyle, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "style", style, entriesTab, false, true, 14);
            Utils.writeJson(outputStream, "substyle", substyle, entriesTab, false, true, 11);
        }
    }

    public struct sEmoteTableTriggerVendorType : iEmoteTableTrigger
    {
        public eEmoteTriggerCategory category;
        public Single probability;
        public eVendorEmoteType vendorType;

        public sEmoteTableTriggerVendorType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
        {
            category = triggerCategory;
            probability = Utils.readSingle(inputFile);
            vendorType = (eVendorEmoteType)Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)category, outputStream);
            Utils.writeSingle(probability, outputStream);
            Utils.writeInt32((int)vendorType, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "vendorType", (int)vendorType, entriesTab, false, true, 9);
            Utils.writeJson(outputStream, "_comment2", vendorType.ToString(), "    ", false, false, 0);
        }
    }

    public struct sEmoteTableTriggerQuestType : iEmoteTableTrigger
    {
        public eEmoteTriggerCategory category;
        public Single probability;
        public string quest;

        public sEmoteTableTriggerQuestType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
        {
            category = triggerCategory;
            probability = Utils.readSingle(inputFile);
            quest = Utils.readStringAndReplaceSpecialCharacters(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)category, outputStream);
            Utils.writeSingle(probability, outputStream);
            Utils.writeString(Utils.restoreStringSpecialCharacters(quest), outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "quest", quest, entriesTab, false, true, 10);
        }
    }

    //public struct sEmoteTableTriggerMsgType : iEmoteTableTrigger
    //{
    //    public eEmoteTriggerCategory category;
    //    public Single probability;
    //    public string msg;

    //    public sEmoteTableTriggerMsgType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
    //    {
    //        category = triggerCategory;
    //        probability = Utils.readSingle(inputFile);
    //        msg = Utils.readStringAndReplaceSpecialCharacters(inputFile);
    //    }

    //    public void writeRaw(StreamWriter outputStream)
    //    {
    //        Utils.writeInt32((int)category, outputStream);
    //        Utils.writeSingle(probability, outputStream);
    //        Utils.writeString(Utils.RestoreStringSpecialCharacters(msg), outputStream);
    //    }

    //    public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
    //    {
    //        string entryStarter = isFirst ? "" : ",";
    //        string entriesTab = $"{tab}\t";

    //        Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
    //        Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
    //        Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
    //        Utils.writeJson(outputStream, "msg", msg, entriesTab, false, true, 12);
    //    }
    //}

    public struct sEmoteTableTriggerClassIDType : iEmoteTableTrigger
    {
        public eEmoteTriggerCategory category;
        public Single probability;
        public int classID;

        public sEmoteTableTriggerClassIDType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
        {
            category = triggerCategory;
            probability = Utils.readSingle(inputFile);
            classID = Utils.readInt32(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)category, outputStream);
            Utils.writeSingle(probability, outputStream);
            Utils.writeInt32(classID, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            cWeenie wcidWeenie = Program.cache9Converter.getWeenie(classID);
            string wcidName = "";

            if (wcidWeenie != null)
                wcidName = wcidWeenie.weenieName;
            wcidName = Utils.removeWcidNameRedundancy(WeenieClassNames.getWeenieClassName(classID), wcidName);

            Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "classID", classID, entriesTab, false, true, 12);
            Utils.writeJson(outputStream, "_comment2", wcidName, "    ", false, false, 0);
        }
    }

    public struct sEmoteTableTriggerMinMaxHealthType : iEmoteTableTrigger
    {
        public eEmoteTriggerCategory category;
        public Single probability;
        public Single minHealth;
        public Single maxHealth;

        public sEmoteTableTriggerMinMaxHealthType(StreamReader inputFile, eEmoteTriggerCategory triggerCategory)
        {
            category = triggerCategory;
            probability = Utils.readSingle(inputFile);
            minHealth = Utils.readSingle(inputFile);
            maxHealth = Utils.readSingle(inputFile);
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32((int)category, outputStream);
            Utils.writeSingle(probability, outputStream);
            Utils.writeSingle(minHealth, outputStream);
            Utils.writeSingle(maxHealth, outputStream);
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            Utils.writeJson(outputStream, "category", (int)category, entriesTab, true, true, 11);
            Utils.writeJson(outputStream, "_comment", category.ToString(), "    ", false, false, 0);
            Utils.writeJson(outputStream, "probability", probability, entriesTab, false, true, 8);
            Utils.writeJson(outputStream, "minhealth", minHealth, entriesTab, false, true, 10);
            Utils.writeJson(outputStream, "maxhealth", maxHealth, entriesTab, false, true, 10);
        }
    }

    public struct sEmoteTableKey
    {
        public int key;
        public List<sEmoteTableEntry> entries;

        public sEmoteTableKey(StreamReader inputFile)
        {
            entries = new List<sEmoteTableEntry>();

            key = Utils.readInt32(inputFile);

            int triggersCount = Utils.readInt32(inputFile);
            for (int currentTrigger = 0; currentTrigger < triggersCount; currentTrigger++)
            {
                entries.Add(new sEmoteTableEntry(inputFile));
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            Utils.writeInt32(key, outputStream);

            Utils.writeInt32(entries.Count, outputStream);
            foreach (sEmoteTableEntry entry in entries)
            {
                entry.writeRaw(outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, tab);
            Utils.writeJson(outputStream, "key", key, entriesTab, true, true, 4);
            outputStream.Write(",\n{0}\"value\": [", entriesTab);

            bool firstEntry = true;
            foreach (sEmoteTableEntry entry in entries)
            {
                entry.writeJson(outputStream, entriesTab, firstEntry);
                if (firstEntry)
                    firstEntry = false;
            }

            outputStream.Write("\n{0}]", entriesTab);
            outputStream.Write("\n{0}}}", tab);
        }
    }

    public struct sEmoteTableEntry
    {
        public iEmoteTableTrigger trigger;
        public List<iEmoteTableAction> actions;

        public sEmoteTableEntry(StreamReader inputFile)
        {
            trigger = new sEmoteTableTriggerEmpty();
            actions = new List<iEmoteTableAction>();
            
            //for (int currentTrigger = 0; currentTrigger < triggersCount; currentTrigger++)
            {
                eEmoteTriggerCategory category = (eEmoteTriggerCategory)Utils.readInt32(inputFile);

                switch (category)
                {
                    case eEmoteTriggerCategory.onPortal:
                    case eEmoteTriggerCategory.onTalk:
                    case eEmoteTriggerCategory.onUse:
                    case eEmoteTriggerCategory.onDeath:
                    case eEmoteTriggerCategory.onKill:
                    case eEmoteTriggerCategory.onAttack:
                    case eEmoteTriggerCategory.onCallReinforcements:
                    case eEmoteTriggerCategory.onSpellLanded:
                    case eEmoteTriggerCategory.onSpellResisted:
                    case eEmoteTriggerCategory.onRetreat:
                    case eEmoteTriggerCategory.onTaunt:
                    case eEmoteTriggerCategory.onUnknownEmoteCategory5:
                    case eEmoteTriggerCategory.onUnknownEmoteCategory2:
                    case eEmoteTriggerCategory.onUnknownEmoteCategory4:
                    case eEmoteTriggerCategory.onUnknownEmoteCategory1:
                    case eEmoteTriggerCategory.onUnknownEmoteCategory3:
                    case eEmoteTriggerCategory.onPickup:
                        {
                            trigger = new sEmoteTableTriggerBasicType(inputFile, category);
                            break;
                        }
                    case eEmoteTriggerCategory.onIdle:
                        {
                            trigger = new sEmoteTableTriggerStyleSubstyleType(inputFile, category);
                            break;
                        }
                    case eEmoteTriggerCategory.onShop:
                        {
                            trigger = new sEmoteTableTriggerVendorType(inputFile, category);
                            break;
                        }
                    case eEmoteTriggerCategory.onQuestFlagCheckSuccess:
                    case eEmoteTriggerCategory.onQuestFlagCheckFailure:
                    case eEmoteTriggerCategory.OnFlagCheckSuccess:
                    case eEmoteTriggerCategory.OnFlagCheckFailure:
                    case eEmoteTriggerCategory.OnFlagCheckSuccess3:
                    case eEmoteTriggerCategory.onEventControllerRunning:
                    case eEmoteTriggerCategory.onEventControllerNotRunning:
                    case eEmoteTriggerCategory.OnFlagCheckFailure2:
                    case eEmoteTriggerCategory.OnFlagCheckSuccess2:
                        {
                            trigger = new sEmoteTableTriggerQuestType(inputFile, category);
                            break;
                        }
                    case eEmoteTriggerCategory.onRefuseItem:
                    case eEmoteTriggerCategory.onReceiveItem:
                        {
                            trigger = new sEmoteTableTriggerClassIDType(inputFile, category);
                            break;
                        }
                    case eEmoteTriggerCategory.onHitPointsThreshold:
                        {
                            trigger = new sEmoteTableTriggerMinMaxHealthType(inputFile, category);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("unknown emote trigger category: {0}", category);
                            Console.WriteLine("--Location: {0}--", inputFile.BaseStream.Position);
                            break;
                        }
                }

                int actionsCount = Utils.readInt32(inputFile);
                for (int i = 0; i < actionsCount; i++)
                {
                    eEmoteActionType actionType = (eEmoteActionType)Utils.readInt32(inputFile);
                    switch (actionType)
                    {
                        case eEmoteActionType.turnToPlayer:
                        case eEmoteActionType.resetHomePosition:
                        case eEmoteActionType.lockFellowship:
                        case eEmoteActionType.useActivationTarget:
                            {
                                actions.Add(new sEmoteTableActionBasicType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.performEmote:
                        case eEmoteActionType.playerPerformEmote:
                            {
                                actions.Add(new sEmoteTableActionMotionType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.textEmote:
                        case eEmoteActionType.textSay:
                        case eEmoteActionType.textTell:
                        case eEmoteActionType.textUnknown1:
                        case eEmoteActionType.textAdmin:
                        case eEmoteActionType.textLocalBroadcast:
                        case eEmoteActionType.textWorldBroadcast:
                        case eEmoteActionType.textPrivate:
                        case eEmoteActionType.textFellowshipTell:
                        case eEmoteActionType.textFellowshipBroadcast:
                        case eEmoteActionType.checkQuestFlag:
                        case eEmoteActionType.updateQuestFlag:
                        case eEmoteActionType.startEvent:
                        case eEmoteActionType.stopEvent:
                        case eEmoteActionType.startEventController:
                        case eEmoteActionType.setQuestFlag:
                        case eEmoteActionType.removeQuestFlag:
                        case eEmoteActionType.gotoEmote:
                        case eEmoteActionType.checkFellowshipQuestFlag:
                        case eEmoteActionType.updateFellowshipQuestFlag:
                        case eEmoteActionType.setFellowshipQuestFlag:
                        case eEmoteActionType.textDirect:
                            {
                                actions.Add(new sEmoteTableActionMsgType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.giveSkillCredit:
                        case eEmoteActionType.giveVitae:
                        case eEmoteActionType.setTitle:
                            {
                                actions.Add(new sEmoteTableActionAmountType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.incrementQuestCounter:
                        case eEmoteActionType.decrementQuestCounter:
                            {
                                actions.Add(new sEmoteTableActionMsgAmountType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.showParticleEffects:
                            {
                                actions.Add(new sEmoteTableActionPScriptType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.playSound:
                            {
                                actions.Add(new sEmoteTableActionSoundType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.rotate:
                        case eEmoteActionType.moveToHomePosition:
                        case eEmoteActionType.moveTo:
                            {
                                actions.Add(new sEmoteTableActionFrameType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.setLifestonePosition:
                            {
                                actions.Add(new sEmoteTableActionMPositionType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.giveItem:
                            {
                                actions.Add(new sEmoteTableActionCProfType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.giveExpFellowship:
                        case eEmoteActionType.giveExp:
                            {
                                actions.Add(new sEmoteTableActionAmountHeroxp64Type(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.increaseSkill:
                        case eEmoteActionType.giveExpSkill:
                            {
                                actions.Add(new sEmoteTableActionAmountStatType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.castSpell:
                        case eEmoteActionType.castSpellInstant:
                        case eEmoteActionType.teachSpell:
                            {
                                actions.Add(new sEmoteTableActionSpellIdType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.giveExpLevelProportional:
                            {
                                actions.Add(new sEmoteTableActionMinMaxPercent64Type(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.giveSkillExpLevelProportional:
                            {
                                actions.Add(new sEmoteTableActionMinMaxPercentSkillType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkBoolStatAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionBoolStatType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkIntStatAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionIntStatType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkFloatStatAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionFloatStatType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkStringStatAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionStringStatType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkAttributeAndSetFlag:
                        case eEmoteActionType.checkUnbuffedAttributeAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionMinMaxAttributeType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkVitalAndSetFlag:
                        case eEmoteActionType.checkRawVitalAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionMinMaxVitalsType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkSkillAndSetFlag:
                        case eEmoteActionType.checkUnbuffedSkillAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionMinMaxSkillsType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkIsSkillTrainedAndSetFlag:
                        case eEmoteActionType.checkIsSkillSpecializedAndSetFlag:
                            {
                                actions.Add(new sEmoteTableActionMsgSkillsType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.checkQuestCompletionCounter:
                            {
                                actions.Add(new sEmoteTableActionMsgMinMax(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.setIntStat:
                        case eEmoteActionType.increaseIntStat:
                        case eEmoteActionType.decreaseIntStat:
                            {
                                actions.Add(new sEmoteTableActionIntStatAmountType(inputFile, actionType));
                                break;
                            }
                        case eEmoteActionType.giveMutatedLoot:
                            {
                                actions.Add(new sEmoteTableActionTreasureType(inputFile, actionType));
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("unknown emote action type: {0}", actionType);
                                Console.WriteLine("--Location: {0}--", inputFile.BaseStream.Position);
                                break;
                            }
                    }
                }
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            trigger.writeRaw(outputStream);

            Utils.writeInt32(actions.Count, outputStream);
            foreach (iEmoteTableAction action in actions)
            {
                action.writeRaw(outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";
            string SubEntriesTab = $"{tab}\t\t";

            outputStream.Write("{0}\n{1}{{", entryStarter, entriesTab);
            trigger.writeJson(outputStream, entriesTab, true);
            outputStream.Write(",\n{0}\"emotes\": [", SubEntriesTab);
            bool firstEntry = true;
            foreach (iEmoteTableAction action in actions)
            {
                action.writeJson(outputStream, SubEntriesTab, firstEntry);
                if (firstEntry)
                    firstEntry = false;
            }
            outputStream.Write("\n{0}]", SubEntriesTab);
            outputStream.Write("\n{0}}}", entriesTab);
        }
    }

    public struct sEmoteTable
    {
        public List<sEmoteTableKey> entries;

        public sEmoteTable(StreamReader inputFile)
        {
            entries = new List<sEmoteTableKey>();

            int sectionHeader = Utils.readInt32(inputFile);

            if (sectionHeader >> 16 == 0x40)
            {
                short amount = (short)sectionHeader;

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new sEmoteTableKey(inputFile));
                }
            }
        }

        public void writeRaw(StreamWriter outputStream)
        {
            int sectionHeader = (short)entries.Count;
            sectionHeader |= 0x40 << 16;

            Utils.writeInt32(sectionHeader, outputStream);

            foreach (sEmoteTableKey entry in entries)
            {
                entry.writeRaw(outputStream);
            }
        }

        public void writeJson(StreamWriter outputStream, string tab, bool isFirst)
        {
            string entryStarter = isFirst ? "" : ",";
            string entriesTab = $"{tab}\t";

            if (entries.Count > 0)
            {
                outputStream.Write("{0}\n{1}\"emoteTable\": [", entryStarter, tab);

                bool firstEntry = true;
                foreach (sEmoteTableKey entry in entries)
                {
                    entry.writeJson(outputStream, entriesTab, firstEntry);
                    if (firstEntry)
                        firstEntry = false;
                }

                outputStream.Write("\n{0}]", tab);
            }
        }
    }
}