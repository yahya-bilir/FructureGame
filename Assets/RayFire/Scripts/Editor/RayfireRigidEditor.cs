﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using RayFire;

namespace RayFireEditor
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireRigid), true)]
    public class RayfireRigidEditor : Editor
    {
        RayfireRigid             rigid;
        ReorderableList          rl_ref_list;  
        static readonly GUIStyle damageStyle = new GUIStyle();
        EditorWindow             uvwEditor;
        Texture2D                uvTexture;
        
        // Foldout
        static bool fld_phy;
        static bool fld_act;
        static bool fld_lim;
        static bool fld_msh;
        static bool fld_prp;
        static bool fld_cls;
        static bool fld_clp;
        static bool fld_ref;
        static bool fld_mat;
        static bool fld_dmg;
        static bool fld_fad;
        static bool fld_res;
        
        // Physic Minimum & Maximum ranges
        const int   solver_min = 1;
        const int   solver_max = 20;
        const float sleep_min  = 0.001f;
        const float sleep_max  = 0.1f;
        const float dampen_min = 0f;
        const float dampen_max = 1f;
        
        // Activation Minimum & Maximum ranges
        const float offset_min     = 0;
        const float offset_max     = 10f;
        const float velocity_min   = 0;
        const float velocity_max   = 5f;
        const float damage_act_min = 0;
        const float damage_act_max = 100f;

        // Limitation Minimum & Maximum ranges
        const float solidity_min = 0;
        const float solidity_max = 10f;
        const int   depth_min    = 0;
        const int   depth_max    = 7;
        const float time_min     = 0.05f;
        const float time_max     = 10f;
        const float size_dml_min = 0.01f;
        const float size_dml_max = 50f;

        // Mesh Minimum & Maximum ranges
        const int   amount_frg_min     = 2;
        const int   amount_frg_max     = 300;
        const int   amount_frg_var_min = 0;
        const int   amount_frg_var_max = 100;
        const float depth_fade_min     = 0.01f;
        const float depth_fade_max     = 1f;
        const float bias_min           = 0f;
        const float bias_max           = 1f;
        const int   seed_frg_min       = 0;
        const int   seed_frg_max       = 99;
        const int   cache_frame_min    = 2;
        const int   cache_frame_max    = 300;
        const int   cache_frags_min    = 1;
        const int   cache_frags_max    = 20;
        const float size_adv_min       = 0;
        const float size_adv_max       = 10f;
        
        // Cluster Minimum & Maximum ranges
        const float cls_flt_area_min   = 0f;
        const float cls_flt_area_max   = 1f;
        const float cls_flt_size_min   = 0;
        const float cls_flt_size_max   = 10f;
        const int   cls_flt_perc_min   = 0;
        const int   cls_flt_perc_max   = 100;
        const int   cls_flt_seed_min   = 0;
        const int   cls_flt_seed_max   = 100;
        const int   cls_ratio_min      = 1;
        const int   cls_ratio_max      = 100;
        const float cls_units_min      = 0;
        const float cls_units_max      = 10f;
        const int   cls_shard_area_min = 0;
        const int   cls_shard_area_max = 100;
        const int   cls_clusters_min   = 0;
        const int   cls_clusters_max   = 40;
        
        // Collapse Minimum & Maximum ranges
        const int   clp_start_min    = 0;
        const int   clp_start_max    = 99;
        const int   clp_end_min      = 1;
        const int   clp_end_max      = 100;
        const int   clp_steps_min    = 1;
        const int   clp_steps_max    = 100;
        const float clp_duration_min = 0;
        const float clp_duration_max = 60f;
        const int   clp_var_min      = 0;
        const int   clp_var_max      = 100;
        const int   clp_seed_min     = 0;
        const int   clp_seed_max     = 99;
        
        // Material Minimum & Maximum ranges
        const float mat_scale_min = 0.01f;
        const float mat_scale_max = 2f;

        // Damage Minimum & Maximum ranges
        const float damage_mlt_min = 0.01f;
        const float damage_mlt_max = 10f;

        // Fade Minimum & Maximum ranges
        const float fade_offset_min = 0;
        const float fade_offset_max = 20f;
        const float fade_time_min   = 1f;
        const float fade_time_max   = 30f;
        const float fade_life_min   = 0f;
        const float fade_life_max   = 90f;
        const float fade_size_min   = 0f;
        const float fade_size_max   = 20f;
        const int   fade_shards_min = 0;
        const int   fade_shards_max = 50;
        
        // Reset Minimum & Maximum ranges
        const float reset_del_min = 0;
        const float reset_del_max = 60f;
        
        // Main Serialized properties
        SerializedProperty sp_mn_ini;
        SerializedProperty sp_mn_obj;
        SerializedProperty sp_mn_sim;
        SerializedProperty sp_mn_dml;

        // Physic Serialized properties
        SerializedProperty sp_phy_mtp;
        SerializedProperty sp_phy_mat;
        SerializedProperty sp_phy_mby;
        SerializedProperty sp_phy_mss;
        SerializedProperty sp_phy_ctp;
        SerializedProperty sp_phy_pln;
        SerializedProperty sp_phy_ign;
        SerializedProperty sp_phy_grv;
        SerializedProperty sp_phy_slv;
        SerializedProperty sp_phy_slt;
        SerializedProperty sp_phy_dmp;
        
        // Activation Serialized properties
        SerializedProperty sp_act_off;
        SerializedProperty sp_act_loc;
        SerializedProperty sp_act_vel;
        SerializedProperty sp_act_dmg;
        SerializedProperty sp_act_act;
        SerializedProperty sp_act_imp;
        SerializedProperty sp_act_con;
        SerializedProperty sp_act_uny;
        SerializedProperty sp_act_atb;
        SerializedProperty sp_act_l;
        SerializedProperty sp_act_lay;

        // limitations Serialized properties
        SerializedProperty sp_lim_col;
        SerializedProperty sp_lim_sol;
        SerializedProperty sp_lim_tag;
        SerializedProperty sp_lim_dep;
        SerializedProperty sp_lim_tim;
        SerializedProperty sp_lim_siz;
        SerializedProperty sp_lim_vis;
        SerializedProperty sp_lim_bld;

        // Mesh Demolition Serialized properties
        SerializedProperty sp_msh_en;
        SerializedProperty sp_msh_am;
        SerializedProperty sp_msh_var;
        SerializedProperty sp_msh_dpf;
        SerializedProperty sp_msh_bias;
        SerializedProperty sp_msh_sd;
        SerializedProperty sp_msh_use;
        SerializedProperty sp_msh_cld;
        SerializedProperty sp_msh_sim;
        SerializedProperty sp_msh_cnv;
        SerializedProperty sp_msh_rnt;
        SerializedProperty sp_msh_rnt_fr;
        SerializedProperty sp_msh_rnt_fg;
        SerializedProperty sp_msh_rnt_sk;
        SerializedProperty sp_msh_adv_rem;
        SerializedProperty sp_msh_adv_dec;
        SerializedProperty sp_msh_adv_slc;
        SerializedProperty sp_msh_adv_cmb;
        SerializedProperty sp_msh_adv_cap;
        SerializedProperty sp_msh_adv_ptr;
        SerializedProperty sp_msh_adv_inp;
        SerializedProperty sp_msh_adv_col;
        SerializedProperty sp_msh_adv_szf;
        SerializedProperty sp_msh_adv_l;
        SerializedProperty sp_msh_adv_lay;
        SerializedProperty sp_msh_adv_t;
        SerializedProperty sp_msh_adv_tag;

        // Cluster Demolition Serialized properties
        SerializedProperty sp_cls_cn;
        SerializedProperty sp_cls_nd;
        SerializedProperty sp_cls_cnt;
        SerializedProperty sp_cls_sim;
        SerializedProperty sp_cls_fl_ar;
        SerializedProperty sp_cls_fl_sz;
        SerializedProperty sp_cls_fl_pr;
        SerializedProperty sp_cls_fl_sd;
        SerializedProperty sp_cls_ds_tp;
        SerializedProperty sp_cls_ds_rt;
        SerializedProperty sp_cls_ds_un;
        SerializedProperty sp_cls_sh_ar;
        SerializedProperty sp_cls_sh_dm;
        SerializedProperty sp_cls_min;
        SerializedProperty sp_cls_max;
        SerializedProperty sp_cls_dml;
        
        // Collapse Serialized properties
        SerializedProperty sp_clp_type;
        SerializedProperty sp_clp_start;
        SerializedProperty sp_clp_end;
        SerializedProperty sp_clp_steps;
        SerializedProperty sp_clp_dur;
        SerializedProperty sp_clp_var;
        SerializedProperty sp_clp_seed;

        // Reference Serialized properties
        SerializedProperty sp_ref_rfs;
        SerializedProperty sp_ref_rnd;
        SerializedProperty sp_ref_act;
        SerializedProperty sp_ref_add;
        SerializedProperty sp_ref_scl;
        SerializedProperty sp_ref_mat;

        // Material Serialized properties
        SerializedProperty sp_mat_scl;
        SerializedProperty sp_mat_inn;
        SerializedProperty sp_mat_out;
        SerializedProperty sp_mat_uve;
        SerializedProperty sp_mat_uvc;
        SerializedProperty sp_mat_uvr;

        // Damage Serialized properties
        SerializedProperty sp_dmg_en;
        SerializedProperty sp_dmg_max;
        SerializedProperty sp_dmg_cur;
        SerializedProperty sp_dmg_col;
        SerializedProperty sp_dmg_mlt;
        SerializedProperty sp_dmg_shr;
        
        // Fade Serialized properties
        SerializedProperty sp_fad_dml;
        SerializedProperty sp_fad_act;
        SerializedProperty sp_fad_ofs;
        SerializedProperty sp_fad_tp;
        SerializedProperty sp_fad_tm;
        SerializedProperty sp_fad_lf_tp;
        SerializedProperty sp_fad_lf_tm;
        SerializedProperty sp_fad_lf_vr;
        SerializedProperty sp_fad_sz;
        SerializedProperty sp_fad_sh;
        
        // Reset Serialized properties
        SerializedProperty sp_res_tm;
        SerializedProperty sp_res_dm;
        SerializedProperty sp_res_cn;
        SerializedProperty sp_res_ac;
        SerializedProperty sp_res_dl;
        SerializedProperty sp_res_ms;
        SerializedProperty sp_res_fr;
        
        private void OnEnable()
        {
            // Get component
            rigid = (RayfireRigid)target;
            
            // Set tag list
            RFUI.SetTags();
            
            // Find Main properties
            sp_mn_ini = serializedObject.FindProperty(nameof(rigid.init));
            sp_mn_obj = serializedObject.FindProperty(nameof(rigid.objTp));
            sp_mn_sim = serializedObject.FindProperty(nameof(rigid.simTp));
            sp_mn_dml = serializedObject.FindProperty(nameof(rigid.dmlTp));
            
            // Find Physic properties
            sp_phy_mtp = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.mt));
            sp_phy_mat = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.ma));
            sp_phy_mby = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.mb));
            sp_phy_mss = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.ms));
            sp_phy_ctp = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.ct));
            sp_phy_pln = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.pc));
            sp_phy_ign = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.ine));
            sp_phy_grv = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.gr));
            sp_phy_slv = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.si));
            sp_phy_slt = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.st));
            sp_phy_dmp = serializedObject.FindProperty(nameof(rigid.physics) + "." + nameof(rigid.physics.dm));
            
            // Find Activation properties
            sp_act_off = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.off));
            sp_act_loc = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.loc));
            sp_act_vel = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.vel));
            sp_act_dmg = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.dmg));
            sp_act_act = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.act));
            sp_act_imp = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.imp));
            sp_act_con = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.con));
            sp_act_uny = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.uny));
            sp_act_atb = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.atb));
            sp_act_l   = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.l));
            sp_act_lay = serializedObject.FindProperty(nameof(rigid.act) + "." + nameof(rigid.act.lay));
            
            // Find limitations properties
            sp_lim_col = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.col));
            sp_lim_sol = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.sol));
            sp_lim_tag = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.tag));
            sp_lim_dep = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.depth));
            sp_lim_tim = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.time));
            sp_lim_siz = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.size));
            sp_lim_vis = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.vis));
            sp_lim_bld = serializedObject.FindProperty(nameof(rigid.lim) + "." + nameof(rigid.lim.bld));
            
            // Find Mesh Demolition properties
            sp_msh_en      = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.engTp));
            sp_msh_am      = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.am));
            sp_msh_var     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.var));
            sp_msh_dpf     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.dpf));
            sp_msh_bias    = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.bias));
            sp_msh_sd      = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.sd));
            sp_msh_use     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.use));
            sp_msh_cld     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.cld));
            sp_msh_sim     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.sim));
            sp_msh_cnv     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.cnv));
            sp_msh_rnt     = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.ch) + "." + nameof(rigid.mshDemol.ch.tp));
            sp_msh_rnt_fr  = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.ch) + "." + nameof(rigid.mshDemol.ch.frm));
            sp_msh_rnt_fg  = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.ch) + "." + nameof(rigid.mshDemol.ch.frg));
            sp_msh_rnt_sk  = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.ch) + "." + nameof(rigid.mshDemol.ch.skp));
            sp_msh_adv_rem = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.rem));
            sp_msh_adv_dec = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.dec));
            sp_msh_adv_slc = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.slc));
            sp_msh_adv_cmb = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.cmb));
            sp_msh_adv_cap = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.cap));
            sp_msh_adv_ptr = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.ptr));
            sp_msh_adv_inp = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.inp));
            sp_msh_adv_col = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.col));
            sp_msh_adv_szf = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.szF));
            sp_msh_adv_l   = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.l));
            sp_msh_adv_lay = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.lay));
            sp_msh_adv_t   = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.t));
            sp_msh_adv_tag = serializedObject.FindProperty(nameof(rigid.mshDemol) + "." + nameof(rigid.mshDemol.prp) + "." + nameof(rigid.mshDemol.prp.tag));
            
            // Find Cluster Demolition properties
            sp_cls_cn    = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.cn));
            sp_cls_nd    = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.nd));
            sp_cls_cnt   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.cnt));
            sp_cls_sim   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.sim));
            sp_cls_fl_ar = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.mAr));
            sp_cls_fl_sz = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.mSz));
            sp_cls_fl_pr = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.pct));
            sp_cls_fl_sd = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.seed));
            sp_cls_ds_tp = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.type));
            sp_cls_ds_rt = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.ratio));
            sp_cls_ds_un = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.units));
            sp_cls_sh_ar = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.sAr));
            sp_cls_sh_dm = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.sDm));
            sp_cls_min   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.mnAm));
            sp_cls_max   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.mxAm));
            sp_cls_dml   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.cDm));
            
            // Find Collapse properties
            sp_clp_type  = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.type));
            sp_clp_start = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.start));
            sp_clp_end   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.end));
            sp_clp_steps = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.steps));
            sp_clp_dur   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.duration));
            sp_clp_var   = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.var));
            sp_clp_seed  = serializedObject.FindProperty(nameof(rigid.clsDemol) + "." + nameof(rigid.clsDemol.collapse) + "." + nameof(rigid.clsDemol.collapse.seed));
            
            // Find Reference properties
            sp_ref_rfs = serializedObject.FindProperty(nameof(rigid.refDemol) + "." + nameof(rigid.refDemol.rfs));
            sp_ref_rnd = serializedObject.FindProperty(nameof(rigid.refDemol) + "." + nameof(rigid.refDemol.rnd));
            sp_ref_act = serializedObject.FindProperty(nameof(rigid.refDemol) + "." + nameof(rigid.refDemol.act));
            sp_ref_add = serializedObject.FindProperty(nameof(rigid.refDemol) + "." + nameof(rigid.refDemol.add));
            sp_ref_scl = serializedObject.FindProperty(nameof(rigid.refDemol) + "." + nameof(rigid.refDemol.scl));
            sp_ref_mat = serializedObject.FindProperty(nameof(rigid.refDemol) + "." + nameof(rigid.refDemol.mat));
            
            // Find Material properties
            sp_mat_scl = serializedObject.FindProperty(nameof(rigid.materials) + "." + nameof(rigid.materials.mScl));
            sp_mat_inn = serializedObject.FindProperty(nameof(rigid.materials) + "." + nameof(rigid.materials.iMat));
            sp_mat_out = serializedObject.FindProperty(nameof(rigid.materials) + "." + nameof(rigid.materials.oMat));
            sp_mat_uve = serializedObject.FindProperty(nameof(rigid.materials) + "." + nameof(rigid.materials.uvE));
            sp_mat_uvc = serializedObject.FindProperty(nameof(rigid.materials) + "." + nameof(rigid.materials.uvC));
            sp_mat_uvr = serializedObject.FindProperty(nameof(rigid.materials) + "." + nameof(rigid.materials.uvR));
            
            // Find Damage properties
            sp_dmg_en  = serializedObject.FindProperty(nameof(rigid.damage) + "." + nameof(rigid.damage.en));
            sp_dmg_max = serializedObject.FindProperty(nameof(rigid.damage) + "." + nameof(rigid.damage.max));
            sp_dmg_cur = serializedObject.FindProperty(nameof(rigid.damage) + "." + nameof(rigid.damage.cur));
            sp_dmg_col = serializedObject.FindProperty(nameof(rigid.damage) + "." + nameof(rigid.damage.col));
            sp_dmg_mlt = serializedObject.FindProperty(nameof(rigid.damage) + "." + nameof(rigid.damage.mlt));
            sp_dmg_shr = serializedObject.FindProperty(nameof(rigid.damage) + "." + nameof(rigid.damage.shr));
            
            // Find Fade properties
            sp_fad_dml   = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.onDemolition));
            sp_fad_act   = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.onActivation));
            sp_fad_ofs   = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.byOffset));
            sp_fad_tp    = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.fadeType));
            sp_fad_tm    = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.fadeTime));
            sp_fad_lf_tp = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.lifeType));
            sp_fad_lf_tm = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.lifeTime));
            sp_fad_lf_vr = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.lifeVariation));
            sp_fad_sz    = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.sizeFilter));
            sp_fad_sh    = serializedObject.FindProperty(nameof(rigid.fading) + "." + nameof(rigid.fading.shardAmount));
            
            // Reset Serialized properties
            sp_res_tm = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.transform));
            sp_res_dm = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.damage));
            sp_res_cn = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.connectivity));
            sp_res_ac = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.action));
            sp_res_dl = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.destroyDelay));
            sp_res_ms = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.mesh));
            sp_res_fr = serializedObject.FindProperty(nameof(rigid.reset) + "." + nameof(rigid.reset.fragments));
            
            // Reorderable list
            rl_ref_list = new ReorderableList(serializedObject, sp_ref_rnd, true, true, true, true)
            {
                drawElementCallback = DrawRefListItems,
                drawHeaderCallback  = DrawRefHeader,
                onAddCallback       = AddRed,
                onRemoveCallback    = RemoveRef
            };

            // Foldout
            if (EditorPrefs.HasKey (TextKeys.rig_fld_phy) == true) fld_phy = EditorPrefs.GetBool (TextKeys.rig_fld_phy);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_act) == true) fld_act = EditorPrefs.GetBool (TextKeys.rig_fld_act);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_lim) == true) fld_lim = EditorPrefs.GetBool (TextKeys.rig_fld_lim);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_msh) == true) fld_msh = EditorPrefs.GetBool (TextKeys.rig_fld_msh);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_cls) == true) fld_cls = EditorPrefs.GetBool (TextKeys.rig_fld_cls);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_clp) == true) fld_clp = EditorPrefs.GetBool (TextKeys.rig_fld_clp);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_ref) == true) fld_ref = EditorPrefs.GetBool (TextKeys.rig_fld_ref);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_mat) == true) fld_mat = EditorPrefs.GetBool (TextKeys.rig_fld_mat);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_dmg) == true) fld_dmg = EditorPrefs.GetBool (TextKeys.rig_fld_dmg);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_fad) == true) fld_fad = EditorPrefs.GetBool (TextKeys.rig_fld_fad);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_res) == true) fld_res = EditorPrefs.GetBool (TextKeys.rig_fld_res);
            if (EditorPrefs.HasKey (TextKeys.rig_fld_prp) == true) fld_prp = EditorPrefs.GetBool (TextKeys.rig_fld_prp);
        }
        
         /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            // Update changed properties
            serializedObject.Update();
            
            // Set style
            RFUI.SetToggleButtonStyle();

            RFUI.Space ();
            GUI_Buttons();
            GUI_Info();
            RFUI.Space ();

            GUI_Main();
            RFUI.Space ();
            GUI_Simulation();
            RFUI.Space ();
            UI_Demolition();
            RFUI.Space ();
    
            RFUI.CaptionBox (TextRig.gui_cap_com);
            GUI_Fade();
            GUI_Reset();
            
            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
         
        /// /////////////////////////////////////////////////////////
        /// Main
        /// /////////////////////////////////////////////////////////

        void GUI_Main()
        {
            RFUI.CaptionBox (TextRig.gui_cap_mn);
            RFUI.Space ();
            RFUI.PropertyField (sp_mn_ini, TextRig.gui_mn_ini);
            RFUI.PropertyField (sp_mn_obj, TextRig.gui_mn_obj);
            GUI_Main_Warning();
        }

        void GUI_Main_Warning()
        {
            if (sp_mn_obj.intValue == (int)ObjectType.MeshRoot ||
                sp_mn_obj.intValue == (int)ObjectType.ConnectedCluster ||
                sp_mn_obj.intValue == (int)ObjectType.NestedCluster)
                if (rigid.transform.childCount < 2)
                    RFUI.HelpBox (TextRig.hlp_obj, MessageType.Warning, true);
            
            if (sp_mn_obj.intValue == (int)ObjectType.SkinnedMesh && rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V2)
                RFUI.HelpBox ("All Skinned Mesh objects and bones should be deep children of this object.", MessageType.None, true);
        }

        /// /////////////////////////////////////////////////////////
        /// Simulation
        /// /////////////////////////////////////////////////////////

        void GUI_Simulation()
        {   
            RFUI.CaptionBox (TextRig.gui_cap_sim);
            RFUI.Space ();
            RFUI.PropertyField (sp_mn_sim, TextRig.gui_mn_sim);
            GUI_Physic();
            RFUI.Space ();
            GUI_Activation();
        }
        
        void GUI_Physic()
        {
            RFUI.Foldout (ref fld_phy, TextKeys.rig_fld_phy, TextPhy.gui_phy.text);
            if (fld_phy == true)
            {
                EditorGUI.indentLevel++;
                
                RFUI.Caption (TextPhy.gui_cap_mat);
                RFUI.PropertyField (sp_phy_mtp, TextPhy.gui_phy_mtp);
                RFUI.PropertyField (sp_phy_mat, TextPhy.gui_phy_mat);
                
                RFUI.Caption (TextPhy.gui_cap_mas);
                RFUI.PropertyField (sp_phy_mby, TextPhy.gui_phy_mby);
                if (rigid.physics.mb == MassType.MassProperty)
                    RFUI.PropertyField (sp_phy_mss, TextPhy.gui_phy_mss);

                RFUI.Caption (TextPhy.gui_cap_col);
                RFUI.PropertyField (sp_phy_ctp, TextPhy.gui_phy_ctp);
                RFUI.PropertyField (sp_phy_pln, TextPhy.gui_phy_pln);
                RFUI.PropertyField (sp_phy_ign, TextPhy.gui_phy_ign);
                
                RFUI.Caption (TextPhy.gui_cap_oth);
                RFUI.PropertyField (sp_phy_grv, TextPhy.gui_phy_grv);
                RFUI.IntSlider (sp_phy_slv, solver_min, solver_max, TextPhy.gui_phy_slv);
                RFUI.Slider (sp_phy_slt, sleep_min, sleep_max, TextPhy.gui_phy_slt);
                
                RFUI.Caption (TextPhy.gui_cap_frg);
                RFUI.Slider (sp_phy_dmp, dampen_min, dampen_max, TextPhy.gui_phy_dmp);
  
                EditorGUI.indentLevel--;
            }
        }
        
        void GUI_Activation()
        {
            if (ActivatableState() == false) 
                return;
            
            RFUI.Foldout (ref fld_act, TextKeys.rig_fld_act, TextAcv.gui_act.text);
            if (fld_act == true)
            {
                EditorGUI.indentLevel++;
                
                RFUI.Caption (TextAcv.gui_cap_act);
                RFUI.Slider (sp_act_off, offset_min, offset_max, TextAcv.gui_act_off);
                if (rigid.act.off > 0)
                    RFUI.PropertyField (sp_act_loc, TextAcv.gui_act_loc);
                RFUI.Slider (sp_act_vel, velocity_min,   velocity_max,   TextAcv.gui_act_vel);
                RFUI.Slider (sp_act_dmg, damage_act_min, damage_act_max, TextAcv.gui_act_dmg);
                RFUI.PropertyField (sp_act_act, TextAcv.gui_act_act);
                RFUI.PropertyField (sp_act_imp, TextAcv.gui_act_imp);
                RFUI.PropertyField (sp_act_con, TextAcv.gui_act_con);
                if (rigid.act.con == true)
                {
                    RFUI.PropertyField (sp_act_uny, TextAcv.gui_act_uny);
                    RFUI.PropertyField (sp_act_atb, TextAcv.gui_act_atb);
                }
                
                RFUI.Caption (TextAcv.gui_cap_pst);
                RFUI.PropertyField (sp_act_l, TextAcv.gui_act_l);
                if (rigid.act.l == true)
                    RFUI.LayerField (sp_act_lay, TextAcv.gui_act_lay);

                EditorGUI.indentLevel--;
            }
        }
        
        bool ActivatableState()
        {
            foreach (RayfireRigid scr in targets)
                if (ActivatableState(scr) == true)
                    return true;
            return false;
        }
        
        static bool ActivatableState(RayfireRigid scr)
        {
            if (scr.simTp == SimType.Inactive || scr.simTp == SimType.Kinematic)
                    return true;
            if (scr.mshDemol.sim == FragSimType.Inactive || scr.mshDemol.sim == FragSimType.Kinematic)
                    return true;
            return false;
        }

        /// /////////////////////////////////////////////////////////
        /// Demolition
        /// /////////////////////////////////////////////////////////

        void UI_Demolition()
        {
            RFUI.CaptionBox (TextRig.gui_cap_dml);
            RFUI.Space ();
            RFUI.PropertyField (sp_mn_dml, TextRig.gui_mn_dml);
            GUI_Limitations();
            GUI_Mesh();
            GUI_Cluster();
            GUI_Reference();
            GUI_Materials();
            GUI_Damage();
        }
        
        bool MeshState()
        {
            foreach (RayfireRigid scr in targets)
                if (MeshState(scr) == true)
                    return true;
            return false;
        }
        
        static bool MeshState(RayfireRigid scr)
        {
            if (scr.objTp == ObjectType.Mesh ||
                scr.objTp == ObjectType.MeshRoot ||
                scr.objTp == ObjectType.SkinnedMesh)
                return true;
            if (scr.clsDemol.sDm == true)
                return true;
            return false;
        }

        /// /////////////////////////////////////////////////////////
        /// Limitations
        /// /////////////////////////////////////////////////////////

        void GUI_Limitations()
        {
            if (rigid.objTp == ObjectType.MeshRoot || rigid.dmlTp != DemolitionType.None)
            {
                RFUI.Foldout (ref fld_lim, TextKeys.rig_fld_lim, TextLim.gui_lim.text);
                if (fld_lim == true)
                {
                    EditorGUI.indentLevel++;

                    RFUI.Caption (TextLim.gui_cap_col);
                    RFUI.PropertyField (sp_lim_col, TextLim.gui_lim_col);
                    if (rigid.lim.col == true)
                        RFUI.Slider (sp_lim_sol, solidity_min, solidity_max, TextLim.gui_lim_sol);
                    if (sp_lim_sol.floatValue == 0)
                        RFUI.HelpBox (TextRig.hlp_sol, MessageType.Info, true);
                    RFUI.TagField (sp_lim_tag, TextLim.gui_lim_tag);
                    
                    RFUI.Caption (TextLim.gui_cap_oth);
                    RFUI.IntSlider (sp_lim_dep, depth_min, depth_max, TextLim.gui_lim_dep);
                    if (sp_lim_dep.intValue == 0)
                        RFUI.HelpBox (TextRig.hlp_limit, MessageType.Info, true);
                    if (sp_lim_dep.intValue >= 3)
                        RFUI.HelpBox (TextRig.hlp_depth, MessageType.Info, true);

                    RFUI.Slider (sp_lim_tim, time_min,     time_max,     TextLim.gui_lim_tim);
                    RFUI.Slider (sp_lim_siz, size_dml_min, size_dml_max, TextLim.gui_lim_siz);
                    RFUI.PropertyField (sp_lim_vis, TextLim.gui_lim_vis);
                    RFUI.PropertyField (sp_lim_bld, TextLim.gui_lim_bld);

                    EditorGUI.indentLevel--;
                }
                RFUI.Space ();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Mesh
        /// /////////////////////////////////////////////////////////
        
        void GUI_Mesh()
        {
            if (MeshState() == true && rigid.dmlTp != DemolitionType.None)
            {
                RFUI.Foldout (ref fld_msh, TextKeys.rig_fld_msh, TextMsh.gui_msh.text);
                if (fld_msh == true)
                {
                    EditorGUI.indentLevel++;
                    RFUI.Caption (TextMsh.gui_cap_frg);
                    if (rigid.mshDemol.use == false || rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V1)
                    {
                        RFUI.IntSlider (sp_msh_am,  amount_frg_min,     amount_frg_max,     TextMsh.gui_msh_am);
                        RFUI.IntSlider (sp_msh_var, amount_frg_var_min, amount_frg_var_max, TextMsh.gui_msh_var);
                        if (sp_lim_dep.intValue != 1)
                            RFUI.Slider (sp_msh_dpf,  depth_fade_min, depth_fade_max, TextMsh.gui_msh_dpf);
                        RFUI.Slider (sp_msh_bias, bias_min,       bias_max,       TextMsh.gui_msh_bias);
                        RFUI.IntSlider (sp_msh_sd, seed_frg_min, seed_frg_max, TextMsh.gui_msh_sd);
                    }

                    RFUI.PropertyField (sp_msh_use, TextMsh.gui_msh_use);
                    RFUI.PropertyField (sp_msh_cld, TextMsh.gui_msh_cld);
                    GUI_Mesh_Advanced();
                    EditorGUI.indentLevel--;
                }
                RFUI.Space ();
            }
        }
        
        void GUI_Mesh_Advanced()
        {
            RFUI.Caption (TextMsh.gui_cap_adv);
            RFUI.PropertyField (sp_msh_sim, TextMsh.gui_msh_sim);
            RFUI.Foldout (ref fld_prp, TextKeys.rig_fld_prp, TextMsh.gui_msh_adv.text);
            if (fld_prp == true)
            {
                EditorGUI.indentLevel++;
                RFUI.PropertyField (sp_msh_en, TextMsh.gui_msh_en);
                if (rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V2)
                    RFUI.PropertyField (sp_msh_adv_slc, TextMsh.gui_msh_adv_slc);
                
                // TODO temp. add support later
                if (rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V1)
                {
                    RFUI.PropertyField (sp_msh_rnt, TextMsh.gui_msh_rnt);
                    if (rigid.mshDemol.ch.tp != CachingType.Disabled)
                    {
                        if (rigid.mshDemol.ch.tp == CachingType.ByFrames)
                            RFUI.IntSlider (sp_msh_rnt_fr, cache_frame_min, cache_frame_max, TextMsh.gui_msh_rnt_fr);
                        if (rigid.mshDemol.ch.tp == CachingType.ByFragmentsPerFrame)
                            RFUI.IntSlider (sp_msh_rnt_fg, cache_frags_min, cache_frags_max, TextMsh.gui_msh_rnt_fg);
                        RFUI.PropertyField (sp_msh_rnt_sk, TextMsh.gui_msh_rnt_sk);
                    }
                }
                
                RFUI.PropertyField (sp_msh_cnv,     TextMsh.gui_msh_cnv);
                RFUI.PropertyField (sp_msh_adv_col, TextMsh.gui_msh_adv_col);
                if (rigid.mshDemol.prp.col != RFColliderType.None)
                    RFUI.Slider (sp_msh_adv_szf, size_adv_min, size_adv_max, TextMsh.gui_msh_adv_szf);
                if (rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V1)
                {
                    RFUI.PropertyField (sp_msh_adv_inp, TextMsh.gui_msh_adv_inp);
                    RFUI.PropertyField (sp_msh_adv_rem, TextMsh.gui_msh_adv_rem);
                    RFUI.PropertyField (sp_msh_adv_dec, TextMsh.gui_msh_adv_dec);
                }
                RFUI.PropertyField (sp_msh_adv_cap, TextMsh.gui_msh_adv_cap);
                if (rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V2)
                {
                    RFUI.PropertyField (sp_msh_adv_cmb, TextMsh.gui_msh_adv_cmb);
                    RFUI.PropertyField (sp_msh_adv_ptr, TextSht.gui_adv_ptr);
                }
                GUI_Layer_Tag();
                
                if (rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V2)
                    RFUI.HelpBox ("WIP. V2 engine works only in Editor Mode for testing purposes.", MessageType.Info, true);
                
                EditorGUI.indentLevel--;
            }
        }
        
        void GUI_Layer_Tag()
        {
            RFUI.PropertyField (sp_msh_adv_l, TextMsh.gui_msh_adv_l);
            if (sp_msh_adv_l.boolValue == false)
                RFUI.LayerField (sp_msh_adv_lay, TextMsh.gui_msh_adv_lay);
            RFUI.PropertyField (sp_msh_adv_t, TextMsh.gui_msh_adv_t);
            if (sp_msh_adv_t.boolValue == false)
                RFUI.TagField (sp_msh_adv_tag, TextMsh.gui_msh_adv_tag);
        }

        /// /////////////////////////////////////////////////////////
        /// Cluster
        /// /////////////////////////////////////////////////////////
        
        void GUI_Cluster()
        {
            if (rigid.IsCluster == true || rigid.mshDemol.cnv == RFDemolitionMesh.ConvertType.ConnectedCluster || rigid.objTp == ObjectType.MeshRoot)
            {
                RFUI.Foldout (ref fld_cls, TextKeys.rig_fld_cls, TextCls.gui_cls.text);
                if (fld_cls == true)
                {
                    EditorGUI.indentLevel++;
                    
                    RFUI.Caption (TextCls.gui_cap_prp);
                    RFUI.PropertyField (sp_cls_cnt, TextCls.gui_cls_cnt);
                    RFUI.PropertyField (sp_cls_sim,  TextCls.gui_cls_sim);
                    
                    RFUI.Caption (TextCls.gui_cap_flt);
                    if (rigid.clsDemol.cnt != ConnectivityType.ByBoundingBox)
                        RFUI.Slider (sp_cls_fl_ar, cls_flt_area_min, cls_flt_area_max, TextCls.gui_cls_fl_ar);
                    RFUI.Slider (sp_cls_fl_sz, cls_flt_size_min, cls_flt_size_max, TextCls.gui_cls_fl_sz);
                    RFUI.IntSlider(sp_cls_fl_pr,  cls_flt_perc_min, cls_flt_perc_max, TextCls.gui_cls_fl_pr);
                    RFUI.IntSlider (sp_cls_fl_sd, cls_flt_seed_min, cls_flt_seed_max, TextCls.gui_cls_fl_sd);

                    RFUI.Caption (TextCls.gui_cap_dml);
                    RFUI.PropertyField (sp_cls_ds_tp, TextCls.gui_cls_ds_tp);
                    if (rigid.clsDemol.type == RFDemolitionCluster.RFDetachType.RatioToSize)
                        RFUI.IntSlider (sp_cls_ds_rt, cls_ratio_min, cls_ratio_max, TextCls.gui_cls_ds_rt);
                    else
                        RFUI.Slider (sp_cls_ds_un, cls_units_min, cls_units_max, TextCls.gui_cls_ds_un);
                    
                    RFUI.Caption (TextCls.gui_cap_shd);
                    RFUI.IntSlider (sp_cls_sh_ar, cls_shard_area_min, cls_shard_area_max, TextCls.gui_cls_sh_ar);
                    RFUI.PropertyField (sp_cls_sh_dm, TextCls.gui_cls_sh_dm);
                    if (sp_cls_sh_dm.boolValue == true)
                        RFUI.HelpBox (TextRig.hlp_sh_dml, MessageType.Info, true);

                    RFUI.Caption (TextCls.gui_cap_cls);
                    RFUI.IntSlider(sp_cls_min, cls_clusters_min, cls_clusters_max, TextCls.gui_cls_min);
                    RFUI.IntSlider(sp_cls_max, cls_clusters_min, cls_clusters_max, TextCls.gui_cls_max);
                    RFUI.PropertyField (sp_cls_dml, TextCls.gui_cls_dml);
                    
                    GUI_Collapse();
                    
                    RFUI.Caption (TextCls.gui_cap_lt);
                    GUI_Layer_Tag();

                    EditorGUI.indentLevel--;
                }
                RFUI.Space ();
            }
        }
        
        void GUI_Collapse()
        {
            RFUI.Caption (TextCls.gui_cap_clp);
            RFUI.Foldout (ref fld_clp, TextKeys.rig_fld_clp, TextCls.gui_cls_prp.text);
            if (fld_clp == true)
            {
                EditorGUI.indentLevel++;
                RFUI.PropertyField (sp_clp_type, TextClp.gui_type);
                RFUI.IntSlider (sp_clp_start, clp_start_min, clp_start_max, TextClp.gui_start);
                RFUI.IntSlider (sp_clp_end,   clp_end_min,   clp_end_max,   TextClp.gui_end);
                RFUI.IntSlider (sp_clp_steps, clp_steps_min, clp_steps_max, TextClp.gui_steps);
                RFUI.Slider (sp_clp_dur, clp_duration_min, clp_duration_max, TextClp.gui_duration);
                if (rigid.clsDemol.collapse.type != RFCollapse.RFCollapseType.Random)
                    RFUI.IntSlider (sp_clp_var, clp_var_min, clp_var_max, TextClp.gui_var);
                RFUI.IntSlider (sp_clp_seed, clp_seed_min, clp_seed_max, TextClp.gui_seed);
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Reference
        /// /////////////////////////////////////////////////////////

        void GUI_Reference()
        {
            if (rigid.dmlTp == DemolitionType.ReferenceDemolition)
            {
                RFUI.Foldout (ref fld_ref, TextKeys.rig_fld_ref, TextRig.gui_ref.text);
                if (fld_ref == true)
                {
                    EditorGUI.indentLevel++;
                    RFUI.Caption (TextRig.gui_cap_prp);
                    RFUI.PropertyField (sp_ref_act, TextRig.gui_ref_act);
                    RFUI.PropertyField (sp_ref_add, TextRig.gui_ref_add);
                    RFUI.PropertyField (sp_ref_scl, TextRig.gui_ref_scl);
                    RFUI.PropertyField (sp_ref_mat, TextRig.gui_ref_mat);
                    RFUI.Caption (TextRig.gui_cap_src);
                    RFUI.PropertyField (sp_ref_rfs, TextRig.gui_ref_rfs);
                    rl_ref_list.DoLayoutList();
                    EditorGUI.indentLevel--;
                }
                RFUI.Space ();
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Materials/UV
        /// /////////////////////////////////////////////////////////

        void GUI_Materials()
        {
            if (MeshState() == false)
                return;
            
            RFUI.Foldout (ref fld_mat, TextKeys.rig_fld_mat, TextRig.gui_mat.text);
            if (fld_mat == true)
            {
                EditorGUI.indentLevel++;
                RFUI.Slider (sp_mat_scl, mat_scale_min, mat_scale_max, TextRig.gui_mat_scl);
                RFUI.PropertyField (sp_mat_inn, TextRig.gui_mat_inn);
                RFUI.PropertyField (sp_mat_out, TextRig.gui_mat_out);
                
                if (rigid.mshDemol.engTp == RayfireShatter.RFEngineType.V2)
                {
                    RFUI.PropertyField (sp_mat_uve, TextSht.gui_mat_uve);
                    if (sp_mat_uve.boolValue == true)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField (sp_mat_uvc, TextSht.gui_mat_uvMin);
                        if (EditorGUI.EndChangeCheck())
                            if (uvwEditor != null)
                                uvwEditor.Repaint();

                        RFUI.Space();

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField (sp_mat_uvr, TextSht.gui_mat_uvMax);
                        if (EditorGUI.EndChangeCheck())
                            if (uvwEditor != null)
                                uvwEditor.Repaint();

                        RFUI.Space();

                        if (GUILayout.Button (TextSht.gui_btn_uvedt, RFUI.buttonStyle, GUILayout.Height (25)))
                            OpenUvEditor();
                    }
                }

                EditorGUI.indentLevel--;
            }
            RFUI.Space ();
        }
        
        void OpenUvEditor()
        {
            // Get inner material
            Material mat = rigid.materials.iMat;
            
            // Use object material if inner material not defined
            if (mat == null)
            {
                Renderer rnd = rigid.gameObject.GetComponent<Renderer>();
                if (rnd != null)
                    mat = rnd.sharedMaterial;
            }
            
            // Get material texture
            if (mat != null)
                uvTexture = (Texture2D)mat.mainTexture;
            
            // Open editor
            uvwEditor = RFUvRegionEditor.ShowWindow (uvTexture, rigid.materials, rigid.name);
        }

        /// /////////////////////////////////////////////////////////
        /// Damage
        /// /////////////////////////////////////////////////////////

        void GUI_Damage()
        {
            RFUI.Foldout (ref fld_dmg, TextKeys.rig_fld_dmg, TextRig.gui_dmg.text);
            if (fld_dmg == true)
            {
                EditorGUI.indentLevel++;
                RFUI.Caption (TextCls.gui_cap_prp);
                RFUI.PropertyField (sp_dmg_en, TextRig.gui_dmg_en);
                if (rigid.objTp == ObjectType.ConnectedCluster)
                    RFUI.PropertyField (sp_dmg_shr, TextRig.gui_dmg_shr);
                RFUI.PropertyField (sp_dmg_max, TextRig.gui_dmg_max);
                
                if (rigid.objTp == ObjectType.ConnectedCluster && rigid.damage.shr == true)
                {
                    // To Damage preview
                }
                else
                    RFUI.PropertyField (sp_dmg_cur, TextRig.gui_dmg_cur);
                
                RFUI.Caption (TextRig.gui_cap_col);
                RFUI.PropertyField (sp_dmg_col, TextRig.gui_dmg_col);
                RFUI.Slider (sp_dmg_mlt, damage_mlt_min, damage_mlt_max, TextRig.gui_dmg_mlt);
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Fade
        /// /////////////////////////////////////////////////////////

        void GUI_Fade()
        {
            RFUI.Foldout (ref fld_fad, TextKeys.rig_fld_fad, TextFad.gui_fad.text);
            if (fld_fad == true)
            {
                EditorGUI.indentLevel++;

                RFUI.Caption (TextFad.gui_cap_ini);
                RFUI.PropertyField (sp_fad_dml, TextFad.gui_fad_dml);
                RFUI.PropertyField (sp_fad_act, TextFad.gui_fad_act);
                RFUI.Slider (sp_fad_ofs, fade_offset_min, fade_offset_max, TextFad.gui_fad_ofs);
                
                RFUI.Caption (TextFad.gui_cap_tp);
                RFUI.PropertyField (sp_fad_tp, TextFad.gui_fad_tp);

                if (rigid.fading.fadeType == FadeType.FallDown ||
                    rigid.fading.fadeType == FadeType.MoveDown ||
                    rigid.fading.fadeType == FadeType.ScaleDown)
                    RFUI.Slider (sp_fad_tm, fade_time_min, fade_time_max, TextFad.gui_fad_tm);
                
                RFUI.Caption (TextFad.gui_cap_lf);
                RFUI.PropertyField (sp_fad_lf_tp, TextFad.gui_fad_lf_tp);
                RFUI.Slider (sp_fad_lf_tm, fade_life_min, fade_life_max, TextFad.gui_fad_lf_tm);
                RFUI.Slider (sp_fad_lf_vr, fade_life_min, fade_life_max, TextFad.gui_fad_lf_vr);
                
                RFUI.Caption (TextFad.gui_cap_flt);
                RFUI.Slider (sp_fad_sz, fade_size_min, fade_size_max, TextFad.gui_fad_sz);
                RFUI.IntSlider (sp_fad_sh, fade_shards_min, fade_shards_max, TextFad.gui_fad_sh);

                EditorGUI.indentLevel--;
            }
            RFUI.Space ();
        }

        /// /////////////////////////////////////////////////////////
        /// Reset
        /// /////////////////////////////////////////////////////////
        
        void GUI_Reset()
        {
            RFUI.Foldout (ref fld_res, TextKeys.rig_fld_res, TextRes.gui_res.text);
            if (fld_res == true )
            {
                EditorGUI.indentLevel++;
                
                RFUI.Caption (TextRes.gui_cap_res);
                RFUI.PropertyField (sp_res_tm, TextRes.gui_res_tm);
                RFUI.PropertyField (sp_res_dm, TextRes.gui_res_dm);
                RFUI.PropertyField (sp_res_cn, TextRes.gui_res_cn);
                
                if (rigid.dmlTp != DemolitionType.None)
                {
                    RFUI.Caption (TextRes.gui_cap_dml);
                    RFUI.PropertyField (sp_res_ac, TextRes.gui_res_ac);
                    if (rigid.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
                        RFUI.Slider (sp_res_dl, reset_del_min, reset_del_max, TextRes.gui_res_dl);
                    
                    if (ReuseState (rigid) == true && rigid.reset.action == RFReset.PostDemolitionType.DeactivateToReset)
                    {
                        RFUI.Caption (TextRes.gui_cap_reu);
                        RFUI.PropertyField (sp_res_ms, TextRes.gui_res_ms);
                        RFUI.PropertyField (sp_res_fr, TextRes.gui_res_fr);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        bool ReuseState(RayfireRigid scr)
        {
            if (scr.objTp == ObjectType.Mesh || scr.objTp == ObjectType.MeshRoot)
                return true;

            if (scr.clsDemol.sDm == true)
                return true;

            return false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Reorderable Reference list
        /// /////////////////////////////////////////////////////////
        
        void DrawRefListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = rl_ref_list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y+2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }
        
        void DrawRefHeader(Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField(rect, TextRig.gui_ref_lst);
        }

        void AddRed(ReorderableList list)
        {
            if (rigid.refDemol.rnd == null)
                rigid.refDemol.rnd = new List<GameObject>();
            rigid.refDemol.rnd.Add (null);
            list.index = list.count;
        }
        
        void RemoveRef(ReorderableList list)
        {
            if (rigid.refDemol.rnd != null)
            {
                rigid.refDemol.rnd.RemoveAt (list.index);
                list.index = list.index - 1;
            }
        }

        
        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////
        
        void GUI_Info()
        {
            // Cache info
            if (rigid.HasMeshes == true)
                GUILayout.Label (TextRig.str_precache + rigid.meshes.Length);
            if (rigid.HasFragments == true)
                GUILayout.Label (TextRig.str_frags + rigid.fragments.Count);

            // Demolition info
            if (Application.isPlaying == true && rigid.enabled == true && rigid.initialized == true && rigid.objTp != ObjectType.MeshRoot)
            {
                // Space
                GUILayout.Space (3);

                // Info
                GUILayout.Label (TextRig.str_info, EditorStyles.boldLabel);

                // Excluded
                if (rigid.physics.exclude == true)
                    GUILayout.Label (TextRig.str_excluded);

                // Size
                GUILayout.Label (TextRig.str_size + rigid.lim.bboxSize);

                // Demolition
                GUILayout.Label (TextRig.str_depth + rigid.lim.currentDepth + "/" + rigid.lim.depth);

                // Damage
                if (rigid.damage.en == true)
                    GUILayout.Label (TextRig.str_damage + rigid.damage.cur + "/" + rigid.damage.max);
                
                // Fading
                if (rigid.fading.state == 1)
                    GUILayout.Label (TextRig.str_fade_pre);
                
                // Fading
                if (rigid.fading.state == 2)
                    GUILayout.Label (TextRig.str_fade_prg);

                // Bad mesh
                if (rigid.mshDemol.badMesh > RayfireMan.inst.advancedDemolitionProperties.badMeshTry)
                    GUILayout.Label (TextRig.str_bad);
            }
            
            // Mesh Root info
            if (rigid.objTp == ObjectType.MeshRoot)
            {
                if (rigid.physics.HasIgnore == true)
                    GUILayout.Label (TextRig.str_ignore + rigid.physics.ign.Count / 2);
            }
            
            // Cluster info
            if (rigid.objTp == ObjectType.NestedCluster || rigid.objTp == ObjectType.ConnectedCluster)
            {
                if (rigid.physics.cc == null)
                    return;
                
                if (rigid.physics.cc.Count == 0)
                    return;

                if (rigid.clsDemol == null)
                    return;

                if (rigid.clsDemol.cluster == null)
                    return;
                
                GUILayout.Label (TextRig.str_cls_coll + rigid.physics.cc.Count);
                if (rigid.objTp == ObjectType.ConnectedCluster)
                {
                    GUILayout.Label (TextRig.str_cls_shards + rigid.clsDemol.cluster.shards.Count + "/" + rigid.clsDemol.am);
                    GUILayout.Label (TextRig.str_integrity + rigid.AmountIntegrity + "%");
                }

                if (rigid.physics.HasIgnore == true)
                    GUILayout.Label (TextRig.str_ignore + rigid.physics.ign.Count / 2);
            }
        }

        void GUI_Buttons()
        {
            if (Application.isPlaying == true)
            {
                if (rigid.initialized == false)
                {
                    if (GUILayout.Button (TextRig.gui_btn_init, GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireRigid != null && (targ as RayfireRigid).initialized == false)
                                (targ as RayfireRigid).Initialize();
                }
                
                // Reuse
                else
                {
                    if (GUILayout.Button (TextRig.gui_btn_reset, GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireRigid != null && (targ as RayfireRigid).initialized == true)
                                (targ as RayfireRigid).ResetRigid();
                }
      
                GUILayout.BeginHorizontal();

                // Demolition
                if (rigid.objTp != ObjectType.MeshRoot)
                {
                    if (GUILayout.Button (TextRig.gui_btn_dml, GUILayout.Height (25)))
                        Demolish();
                }

                // Activate
                if (rigid.simTp == SimType.Inactive || rigid.simTp == SimType.Kinematic)
                {
                    if (GUILayout.Button (TextRig.gui_btn_act, GUILayout.Height (25)))
                        Activate();
                }
                
                // Fade
                if (GUILayout.Button (TextRig.gui_btn_fad,     GUILayout.Height (25))) 
                    Fade();
                
                EditorGUILayout.EndHorizontal();
            }
            
            // Setup
            if (Application.isPlaying == false)
            {
                // Clusters
                if (rigid.objTp == ObjectType.MeshRoot)
                {
                    GUILayout.Label (TextRig.str_mesh_root, EditorStyles.boldLabel);
                    SetupUI();
                }
            }
            
            // Clusters
            if (rigid.IsCluster == true)
            {
                GUILayout.Label (TextRig.str_cls, EditorStyles.boldLabel);
                if (Application.isPlaying == false)
                    SetupUI();
                GUILayout.Space (1);
                GUI_Cluster_Preview ();
                if (Application.isPlaying == true)
                    ClusterCollapseUI();
            }
        }
        
        void Demolish()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                        (targ as RayfireRigid).DemolishForced();
        }
        
        void Activate()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                        if ((targ as RayfireRigid).simTp == SimType.Inactive || (targ as RayfireRigid).simTp == SimType.Kinematic)
                            (targ as RayfireRigid).Activate();
        }
        
        void Fade()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                        (targ as RayfireRigid).Fade();
        }
        
        void SetupUI()
        {
            GUILayout.BeginHorizontal();
             
            if (GUILayout.Button (TextRig.gui_btn_edt_setup, RFUI.buttonStyle, GUILayout.Height (25)))
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                    {
                        (targ as RayfireRigid).EditorSetup();
                        RFUI.SetDirty ((targ as RayfireRigid).gameObject); 
                    }
            
            if (GUILayout.Button (TextRig.gui_btn_edt_reset, RFUI.buttonStyle, GUILayout.Height (25)))
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                    {
                        (targ as RayfireRigid).ResetSetup();
                        RFUI.SetDirty ((targ as RayfireRigid).gameObject); 
                    }

            EditorGUILayout.EndHorizontal();
        }
        
        /// /////////////////////////////////////////////////////////
        /// Cluster UI
        /// /////////////////////////////////////////////////////////
        
        void ClusterCollapseUI()
        {
            if (rigid.objTp == ObjectType.ConnectedCluster)
            {
                GUILayout.Label ("  Collapse", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();

                GUILayout.Label (TextClp.str_area, GUILayout.Width (55));

                // Start check for slider change
                EditorGUI.BeginChangeCheck();
                rigid.clsDemol.cluster.areaCollapse = EditorGUILayout.Slider (rigid.clsDemol.cluster.areaCollapse,
                    rigid.clsDemol.cluster.minimumArea, rigid.clsDemol.cluster.maximumArea);
                if (EditorGUI.EndChangeCheck() == true)
                    if (Application.isPlaying == true)
                        RFCollapse.AreaCollapse (rigid, rigid.clsDemol.cluster.areaCollapse);

                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label (TextClp.str_size, GUILayout.Width (55));

                // Start check for slider change
                EditorGUI.BeginChangeCheck();
                rigid.clsDemol.cluster.sizeCollapse = EditorGUILayout.Slider (rigid.clsDemol.cluster.sizeCollapse,
                    rigid.clsDemol.cluster.minimumSize, rigid.clsDemol.cluster.maximumSize);
                if (EditorGUI.EndChangeCheck() == true)
                    if (Application.isPlaying == true)
                        RFCollapse.SizeCollapse (rigid, rigid.clsDemol.cluster.sizeCollapse);

                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label (TextClp.str_rand, GUILayout.Width (55));

                // Start check for slider change
                EditorGUI.BeginChangeCheck();
                rigid.clsDemol.cluster.randomCollapse = EditorGUILayout.IntSlider (rigid.clsDemol.cluster.randomCollapse, 0, 100);
                if (EditorGUI.EndChangeCheck() == true)
                    RFCollapse.RandomCollapse (rigid, rigid.clsDemol.cluster.randomCollapse);

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button (TextRig.gui_btn_clp_start, GUILayout.Height (25)))
                    if (Application.isPlaying)
                        foreach (var targ in targets)
                            if (targ as RayfireRigid != null)
                                RFCollapse.StartCollapse (targ as RayfireRigid);
            }
        }
        
        void GUI_Cluster_Preview()
        {
            if (rigid.objTp == ObjectType.ConnectedCluster)
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                RFUI.Toggle (sp_cls_cn, TextRig.gui_btn_conns, RFUI.color_btn_blue);
                RFUI.Toggle (sp_cls_nd, TextRig.gui_btn_nodes, RFUI.color_btn_blue);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                    SceneView.RepaintAll();
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Draw
        /// /////////////////////////////////////////////////////////

        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireRigid targ, GizmoType gizmoType)
        {
            // Missing shards
            if (RFCluster.IntegrityCheck (targ.clsDemol.cluster) == false)
                Debug.Log (TextRig.rfRig + targ.name + TextRig.str_mis_shards, targ.gameObject);
            
            ClusterDraw (targ);
        }
        
        // CLuster connection and nodes viewport preview
        static void ClusterDraw(RayfireRigid targ)
        {
            if (targ.objTp == ObjectType.ConnectedCluster)
            {
                // Damage style
                damageStyle.fontSize         = 15;
                damageStyle.normal.textColor = Color.red;
                
                if (targ.clsDemol.cluster != null && targ.clsDemol.cluster.shards.Count > 0)
                {
                    // Reinit connections
                    if (targ.clsDemol.cluster.initialized == false)
                        RFCluster.InitCluster (targ, targ.clsDemol.cluster);
                    
                    // Draw
                    for (int i = 0; i < targ.clsDemol.cluster.shards.Count; i++)
                    {
                        if (targ.clsDemol.cluster.shards[i].tm != null)
                        {
                            // Damage
                            if (targ.damage.shr == true)
                            {
                                if (targ.clsDemol.cluster.shards[i].dm > 0)
                                {
                                    Vector3 pos = targ.clsDemol.cluster.shards[i].tm.position;
                                    Handles.Label (pos, targ.clsDemol.cluster.shards[i].dm.ToString ("F1"), damageStyle);
                                }
                            }

                            // Set color
                            if (targ.clsDemol.cluster.shards[i].uny == false)
                            {
                                Gizmos.color = targ.clsDemol.cluster.shards[i].nIds.Count > 0 
                                    ? Color.blue 
                                    : Color.gray;
                            }
                            else
                                Gizmos.color = targ.clsDemol.cluster.shards[i].act == true ? Color.magenta : Color.red;

                            // Nodes
                            if (targ.clsDemol.nd == true) 
                                Gizmos.DrawWireSphere (targ.clsDemol.cluster.shards[i].tm.position, targ.clsDemol.cluster.shards[i].sz / 12f);
                            
                            // Connections
                            if (targ.clsDemol.cn == true)
                                if (targ.clsDemol.cluster.shards[i].neibShards != null)
                                    for (int j = 0; j < targ.clsDemol.cluster.shards[i].neibShards.Count; j++)
                                        if (targ.clsDemol.cluster.shards[i].neibShards[j].tm != null)
                                            Gizmos.DrawLine (targ.clsDemol.cluster.shards[i].tm.position, targ.clsDemol.cluster.shards[i].neibShards[j].tm.position);
                        }
                    }
                }
            }
        }
    }
}